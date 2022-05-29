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
        private const string _artistTag = "yohane";
        private const string _characterTag = "lumine_(genshin_impact)";
        private const string _invalidTag = "ThisDoesNotExist";
        private const string _copyrightTag = "kantai_collection";
        private const string _generalTag = "thighhighs";
        private const string _metadataTag = "highres";

        public class GetByName
        {
            [Test]
            [TestCase(typeof(Danbooru), _artistTag, TagKind.Artist)]
            [TestCase(typeof(Danbooru), _characterTag, TagKind.Character)]
            [TestCase(typeof(Danbooru), _copyrightTag, TagKind.Copyright)]
            [TestCase(typeof(Danbooru), _generalTag, TagKind.General)]
            [TestCase(typeof(Danbooru), _metadataTag, TagKind.Metadata)]

            [TestCase(typeof(Gelbooru), _artistTag, TagKind.Artist)]
            [TestCase(typeof(Gelbooru), _characterTag, TagKind.Character)]
            [TestCase(typeof(Gelbooru), _copyrightTag, TagKind.Copyright)]
            [TestCase(typeof(Gelbooru), _generalTag, TagKind.General)]
            [TestCase(typeof(Gelbooru), _metadataTag, TagKind.Metadata)]

            [TestCase(typeof(Konachan), _artistTag, TagKind.Artist)]
            [TestCase(typeof(Konachan), _characterTag, TagKind.Character)]
            [TestCase(typeof(Konachan), _copyrightTag, TagKind.Copyright)]
            [TestCase(typeof(Konachan), _generalTag, TagKind.General)]
            [TestCase(typeof(Konachan), _metadataTag, TagKind.Metadata)]

            [TestCase(typeof(Yandere), _artistTag, TagKind.Artist)]
            [TestCase(typeof(Yandere), _characterTag, TagKind.Character)]
            [TestCase(typeof(Yandere), _copyrightTag, TagKind.Copyright)]
            [TestCase(typeof(Yandere), _generalTag, TagKind.General)]
            // yande.re doesn't have metadata tags at all it seems.
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
            public void GetByName_Cancellation(Type booruType, string name = _copyrightTag)
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
