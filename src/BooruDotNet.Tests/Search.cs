using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using BooruDotNet.Tests.Helpers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Search
    {
        private static readonly DanbooruService _danbooru = new DanbooruService();
        private const int _testPostId = 123456;

        public class ByUri
        {
            private readonly Uri _testUri = new Uri("https://cdn.donmai.us/preview/47/fa/47faa37362c3eca37fb9cd7dab3545b8.jpg");

            [Test]
            public async Task SearchByUri_Success()
            {
                var results = await _danbooru.SearchByAsync(_testUri);
                var firstResult = results.First();

                Assert.IsTrue(firstResult.Post.ID == _testPostId);
            }

            [Test]
            public void SearchByUri_Cancellation()
            {
                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelpers.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(() => _danbooru.SearchByAsync(_testUri, tokenSource.Token));
            }
        }

        public class ByFile
        {
            private const string _testFilePath = @".\Images\47faa37362c3eca37fb9cd7dab3545b8.jpg";

            [Test]
            public async Task SearchByFile_Success()
            {
                using var file = File.OpenRead(_testFilePath);
                var results = await _danbooru.SearchByAsync(file);
                var firstResult = results.First();

                Assert.IsTrue(firstResult.Post.ID == _testPostId);
            }

            [Test]
            public void SearchByFile_Cancellation()
            {
                using var file = File.OpenRead(_testFilePath);
                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelpers.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(() => _danbooru.SearchByAsync(file, tokenSource.Token));
            }
        }
    }
}
