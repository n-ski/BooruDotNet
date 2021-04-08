using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BooruDotNet.Extensions;
using Validation;

namespace BooruDotNet.Downloaders
{
    public abstract class DownloaderBase<T> : IDownloader where T : notnull
    {
        private DownloaderOptions? _userOptions;
        private static readonly DownloaderOptions _defaultOptions = new DownloaderOptions();

        protected DownloaderBase(HttpClient httpClient)
        {
            HttpClient = Requires.NotNull(httpClient, nameof(httpClient));
        }

        [AllowNull]
        public DownloaderOptions Options
        {
            get => _userOptions ?? _defaultOptions;
            set => _userOptions = value;
        }

        protected HttpClient HttpClient { get; }

        protected static object FileMoveLock { get; } = new object();

        public virtual async Task<FileInfo> DownloadAsync(T item, string targetDirectory,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNullAllowStructs(item, nameof(item));
            Requires.Argument(Directory.Exists(targetDirectory), nameof(targetDirectory), "Directory does not exist.");

            string targetFilePath = Path.Combine(
                targetDirectory,
                GetFileName(item));

            if (Options.OverwriteExisting || !File.Exists(targetFilePath))
            {
                string tempFilePath = GetRandomTempFilePath();
                Uri downloadUri = GetDownloadUri(item);

                using (HttpResponseMessage response = await HttpClient.GetAsync(
                    downloadUri,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    using Stream tempFileStream = File.Create(tempFilePath);
                    await response.Content.CopyToAsync(tempFileStream).ConfigureAwait(false);
                }

                MoveFileSafe(tempFilePath, targetFilePath);
            }
            else
            {
                Logger.Debug($"File '{targetFilePath}' already exists.", this);
            }

            return new FileInfo(targetFilePath);
        }

        public virtual async IAsyncEnumerable<FileInfo> DownloadAsync(
            IEnumerable<T> items, string targetDirectory,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(items, nameof(items));
            Requires.Argument(Directory.Exists(targetDirectory), nameof(targetDirectory), "Directory does not exist.");

            Func<T, Task<FileInfo?>> downloadItemAsync;

            if (Options.IgnoreErrors)
            {
                downloadItemAsync = async item =>
                {
                    try
                    {
                        return await DownloadAsync(item, targetDirectory, cancellationToken).ConfigureAwait(false);
                    }
                    // Ignore all errors except task cancellation. Use "when" clause to
                    // match both TaskCanceled and OperationCanceled exceptions.
                    catch (Exception ex) when (ex is OperationCanceledException == false)
                    {
                        return null;
                    }
                };
            }
            else
            {
                downloadItemAsync = item => DownloadAsync(item, targetDirectory, cancellationToken)!;
            }

            int batchSize = Options.BatchSize;

            if (batchSize > 1)
            {
                var transformBlock = new TransformBlock<T, FileInfo?>(
                    downloadItemAsync,
                    new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = batchSize });

                foreach (T item in items)
                {
                    transformBlock.Post(item);
                }

                transformBlock.Complete();

                while (await transformBlock.OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
                {
                    FileInfo? file = await transformBlock.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    if (file is object)
                    {
                        yield return file;
                    }
                }
            }
            else
            {
                foreach (T item in items)
                {
                    FileInfo? file = await downloadItemAsync(item).ConfigureAwait(false);

                    if (file is object)
                    {
                        yield return file;
                    }
                }
            }
        }

        protected abstract Uri GetDownloadUri(T item);

        protected virtual string GetFileName(T item)
        {
            return GetDownloadUri(item).GetFileName();
        }

        private static void MoveFileSafe(string sourcePath, string destinationPath)
        {
            lock (FileMoveLock)
            {
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                File.Move(sourcePath, destinationPath);
            }
        }

        // This doesn't create empty temp files, unlike Path.GetTempFileName().
        private static string GetRandomTempFilePath()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        Task<FileInfo> IDownloader.DownloadAsync(object item, string targetDirectory,
            CancellationToken cancellationToken)
        {
            if (item is T typeItem)
            {
                return DownloadAsync(typeItem, targetDirectory, cancellationToken);
            }

            throw new ArgumentException($"Object is not of type '{typeof(T)}'.");
        }

        IAsyncEnumerable<FileInfo> IDownloader.DownloadAsync(IEnumerable items, string targetDirectory,
            CancellationToken cancellationToken)
        {
            Requires.NotNull(items, nameof(items));

            IEnumerable<T> typeItems = items.OfType<T>();

            Requires.Argument(typeItems.Any(), nameof(items), $"There is no objects of type '{typeof(T)}' to download.");

            return DownloadAsync(typeItems, targetDirectory, cancellationToken);
        }
    }
}
