using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Tags;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Tags
    {
        private const string _normalTag = "kantai_collection";
        private const string _weirdTag = ";d";
        private const string _invalidTag = "ThisDoesNotExist";

        public class GetByName
        {
            [Test]
            [TestCase(typeof(Danbooru), _normalTag, TagKind.Copyright)]
            [TestCase(typeof(Gelbooru), _normalTag, TagKind.Copyright)]
            [TestCase(typeof(Konachan), _normalTag, TagKind.Copyright)]
            [TestCase(typeof(Yandere), _normalTag, TagKind.Copyright)]
            [TestCase(typeof(Danbooru), _weirdTag, TagKind.General)]
            [TestCase(typeof(Gelbooru), _weirdTag, TagKind.General)]
            [TestCase(typeof(Konachan), _weirdTag, TagKind.General)]
            [TestCase(typeof(Yandere), _weirdTag, TagKind.General)]
            public async Task GetByName_Success(Type booruType, string name, TagKind kind)
            {
                var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

                var tag = await booru.GetTagAsync(name);

                Assert.AreEqual(name, tag.Name);
                Assert.AreEqual(kind, tag.Kind);
            }

            [Test]
            [TestCase(typeof(Danbooru))]
            [TestCase(typeof(Gelbooru))]
            [TestCase(typeof(Konachan))]
            [TestCase(typeof(Yandere))]
            public void GetByName_Cancellation(Type booruType, string name = _normalTag)
            {
                // IMPORTANT: create raw instance here to not mess with other tests.
                // See TagsCache.cs.
                var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

                Assert.ThrowsAsync<TaskCanceledException>(() => booru.GetTagAsync(name, tokenSource.Token));
            }

            [Test]
            [TestCase(typeof(Danbooru))]
            [TestCase(typeof(Gelbooru))]
            [TestCase(typeof(Konachan))]
            [TestCase(typeof(Yandere))]
            // Case-sensitive search.
            [TestCase(typeof(Konachan), "Pantyhose")]
            [TestCase(typeof(Yandere), "Kantai_Collection")]
            public void GetByName_Fail(Type booruType, string name = _invalidTag)
            {
                var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

                Assert.ThrowsAsync<InvalidTagNameException>(() => booru.GetTagAsync(name));
            }
        }
    }
}
