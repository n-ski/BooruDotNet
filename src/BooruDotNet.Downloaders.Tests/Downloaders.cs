using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Caching;
using BooruDotNet.Namers;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Downloaders.Tests
{
    public class Downloaders
    {
        private static readonly PostCache _postCache = BooruHelper.PostCaches[typeof(Danbooru.Danbooru)];
        private static readonly IPostNamer _namer = new HashNamer();
        private static readonly PostDownloader _downloader = new PostDownloader(BooruHelper.HttpClient, _namer);
        private static readonly MD5 _md5 = MD5.Create();
        private static readonly string _tempDirectoryPath = Path.GetTempPath();

        public class Posts
        {
            [Test]
            [TestCase(4067797)]
            [TestCase(4166623)]
            [TestCase(4171159)]
            public async Task DownloadPost_Success(int postId)
            {
                var post = await _postCache.GetPostAsync(postId);
                var file = await _downloader.DownloadAsync(post, _tempDirectoryPath);

                try
                {
                    CheckMatchingHash(file, post.Hash);
                }
                finally
                {
                    file.Delete();
                }
            }

            private static void CheckMatchingHash(FileInfo file, string hash)
            {
                using var fileStream = file.OpenRead();
                var fileHash = GetHash(fileStream);

                Assert.AreEqual(hash, fileHash);
            }

            [Test]
            [TestCase(4067797)]
            [TestCase(4166623)]
            [TestCase(4171159)]
            public async Task DownloadPost_Cancellation(int postId)
            {
                var post = await _postCache.GetPostAsync(postId);

                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(
                    () => _downloader.DownloadAsync(post, _tempDirectoryPath, tokenSource.Token));
            }
        }

        private static string GetHash(Stream stream)
        {
            var hashBytes = _md5.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", null).ToLowerInvariant();
        }
    }
}
