using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Tags;
using BooruDotNet.Tests.Helpers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Tags
    {
        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public async Task GetByName_Success(Type booruType, string name = "pantyhose")
        {
            var booru = BooruHelpers.TagCaches[booruType];

            var tag = await booru.GetTagAsync(name);

            Assert.AreEqual(name, tag.Name);
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public void GetByName_Cancellation(Type booruType, string name = "pantyhose")
        {
            // IMPORTANT: create raw instance here to not mess with other tests.
            // See TagsCache.cs.
            var booru = BooruHelpers.Create<IBooruTagByName>(booruType);

            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(BooruHelpers.TaskCancellationDelay);

            Assert.ThrowsAsync<TaskCanceledException>(() => booru.GetTagAsync(name, tokenSource.Token));
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public void GetByName_Fail(Type booruType)
        {
            var booru = BooruHelpers.TagCaches[booruType];

            Assert.ThrowsAsync<InvalidTagNameException>(() => booru.GetTagAsync("ThisDoesNotExist"));
        }
    }
}
