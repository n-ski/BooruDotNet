using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BooruDotNet.Extensions;
using Easy.Common;

namespace BooruDotNet.Downloaders
{
    public abstract class DownloaderBase<T> : IDownloader where T : notnull
    {
        private int _batchSize;

        protected DownloaderBase(HttpClient httpClient)
        {
            HttpClient = Ensure.NotNull(httpClient, nameof(httpClient));
            BatchSize = 1;
        }

        public int BatchSize
        {
            get => _batchSize;
            set => _batchSize = Math.Max(1, value);
        }

        protected HttpClient HttpClient { get; }

        protected static object FileMoveLock { get; } = new object();

        public virtual async Task<FileInfo> DownloadAsync(T item, string targetDirectory,
            CancellationToken cancellationToken = default)
        {
            Ensure.Exists(new DirectoryInfo(targetDirectory));
            Error.If<ArgumentNullException>(item is null, nameof(item));

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

            string targetFilePath = Path.Combine(
                targetDirectory,
                GetFileName(item));

            MoveFileSafe(tempFilePath, targetFilePath);

            return new FileInfo(targetFilePath);
        }

        public virtual async IAsyncEnumerable<FileInfo> DownloadAsync(
            IEnumerable<T> items, string targetDirectory,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Ensure.Exists(new DirectoryInfo(targetDirectory));
            Ensure.That(items.Any(), "There is no items to download.");

            System.Diagnostics.Debug.WriteLine($"Begin download {items.Count()} post(s) w/ {BatchSize} thread(s).", GetType().Name);

            if (BatchSize > 1)
            {
                var transformBlock = new TransformBlock<T, FileInfo>(
                    item => DownloadAsync(item, targetDirectory, cancellationToken),
                    new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = BatchSize });

                foreach (T item in items)
                {
                    transformBlock.Post(item);
                }

                transformBlock.Complete();

                while (await transformBlock.OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
                {
                    yield return await transformBlock.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            // Fast path.
            else
            {
                foreach (T item in items)
                {
                    yield return await DownloadAsync(item, targetDirectory, cancellationToken).ConfigureAwait(false);
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
            var typeItems = items.OfType<T>();

            Ensure.That(typeItems.Any(), $"There is no objects of type '{typeof(T)}' to download.");

            return DownloadAsync(typeItems, targetDirectory, cancellationToken);
        }
    }
}
