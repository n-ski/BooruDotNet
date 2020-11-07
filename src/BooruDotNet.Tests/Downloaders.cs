using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Downloaders;
using BooruDotNet.Namers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Downloaders
    {
        private static readonly Danbooru _booru = new Danbooru();
        private static readonly IPostNamer _namer = new HashNamer();
        private static readonly PostDownloader _downloader = new PostDownloader(SingletonHttpClient.Instance, _namer);
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
                var post = await _booru.GetPostAsync(postId);
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
        }

        [Test]
        [TestCase(4067797)]
        [TestCase(4166623)]
        [TestCase(4171159)]
        public async Task DownloadCancellation_Success(int postId)
        {
            var post = await _booru.GetPostAsync(postId);

            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(50);

            Assert.ThrowsAsync<TaskCanceledException>(
                () => _downloader.DownloadAsync(post, _tempDirectoryPath, tokenSource.Token));
        }

        private static string GetHash(Stream stream)
        {
            var hashBytes = _md5.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", null).ToLowerInvariant();
        }
    }
}
