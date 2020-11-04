using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        }

        public int BatchSize
        {
            get => _batchSize;
            set => _batchSize = Math.Max(1, value);
        }

        protected HttpClient HttpClient { get; }

        protected static object FileMoveLock { get; } = new object();

        public virtual async Task DownloadAsync(T item, string targetDirectory, CancellationToken cancellationToken = default)
        {
            Ensure.Exists(new DirectoryInfo(targetDirectory));
            Error.If<ArgumentNullException>(item is null, nameof(item));

            string tempFilePath = Path.GetTempFileName();
            Uri downloadUri = GetDownloadUri(item);

            using (HttpResponseMessage response = await HttpClient.GetAsync(
                downloadUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                using Stream tempFileStream = File.Create(tempFilePath);
                await response.Content.CopyToAsync(tempFileStream);
            }

            string targetFilePath = Path.Combine(
                targetDirectory,
                GetFileName(item));

            MoveFileSafe(tempFilePath, targetFilePath);
        }

        public virtual async Task DownloadAsync(IEnumerable<T> items, string targetDirectory, CancellationToken cancellationToken = default)
        {
            Ensure.Exists(new DirectoryInfo(targetDirectory));
            Ensure.That(items.Any(), "There is no items to download.");

            await Task.Run(() =>
            {
                Partitioner
                    .Create(items, EnumerablePartitionerOptions.NoBuffering)
                    .AsParallel()
                    .WithDegreeOfParallelism(BatchSize)
                    .WithCancellation(cancellationToken)
                    .ForAll(item =>
                    {
                        DownloadAsync(item, targetDirectory, cancellationToken).Wait();
                    });
            }, cancellationToken);
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

        Task IDownloader.DownloadAsync(object item, string targetDirectory, CancellationToken cancellationToken)
        {
            if (item is T typeItem)
            {
                return DownloadAsync(typeItem, targetDirectory, cancellationToken);
            }

            throw new ArgumentException($"Object is not of type '{typeof(T)}'.");
        }

        Task IDownloader.DownloadAsync(IEnumerable items, string targetDirectory, CancellationToken cancellationToken)
        {
            var typeItems = items.OfType<T>();

            Ensure.That(typeItems.Any(), $"There is no objects of type '{typeof(T)}' to download.");

            return DownloadAsync(typeItems, targetDirectory, cancellationToken);
        }
    }
}
