using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using BooruDotNet.Search.Tests.Helpers;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Search.Tests
{
    public class Search
    {
        private const int _testPostId = 123456;

        public class ByUri
        {
            private static readonly Uri _testUri = new Uri("https://cdn.donmai.us/preview/47/fa/47faa37362c3eca37fb9cd7dab3545b8.jpg");

            [Test]
            [TestCase(typeof(DanbooruService))]
            [TestCase(typeof(DanbooruIqdbService))]
            public async Task SearchByUri_Success(Type serviceType)
            {
                var service = ServiceHelper.CreateService<ISearchByUri>(serviceType);

                var results = await service.SearchByAsync(_testUri);
                var firstResult = results.First();

                Assert.IsTrue(ExtractPostIdFrom(firstResult) == _testPostId);
            }

            [Test]
            [TestCase(typeof(DanbooruService))]
            [TestCase(typeof(DanbooruIqdbService))]
            public void SearchByUri_Cancellation(Type serviceType)
            {
                var service = ServiceHelper.CreateService<ISearchByUri>(serviceType);

                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(() => service.SearchByAsync(_testUri, tokenSource.Token));
            }
        }

        public class ByFile
        {
            private const string _testFilePath = @".\Images\47faa37362c3eca37fb9cd7dab3545b8.jpg";

            [Test]
            [TestCase(typeof(DanbooruService))]
            [TestCase(typeof(DanbooruIqdbService))]
            public async Task SearchByFile_Success(Type serviceType)
            {
                var service = ServiceHelper.CreateService<ISearchByFile>(serviceType);

                using var file = File.OpenRead(_testFilePath);
                var results = await service.SearchByAsync(file);
                var firstResult = results.First();

                Assert.IsTrue(ExtractPostIdFrom(firstResult) == _testPostId);
            }

            [Test]
            [TestCase(typeof(DanbooruService))]
            [TestCase(typeof(DanbooruIqdbService))]
            public void SearchByFile_Cancellation(Type serviceType)
            {
                var service = ServiceHelper.CreateService<ISearchByFile>(serviceType);

                using var file = File.OpenRead(_testFilePath);
                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(() => service.SearchByAsync(file, tokenSource.Token));
            }
        }

        private static int ExtractPostIdFrom(IResult result)
        {
            if (result is IResultWithPost resultWithPost)
            {
                return resultWithPost.Post.ID.Value;
            }
            else
            {
                // TODO: only works for Danbooru now.
                return int.Parse(result.Source.Segments[^1]);
            }
        }
    }
}
