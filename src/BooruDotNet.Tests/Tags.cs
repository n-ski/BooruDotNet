using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Tags;
using NUnit.Framework;

namespace BooruDotNet.Tests;

public class Tags
{
    private const string ArtistTag = "yohane";
    private const string CharacterTag = "lumine_(genshin_impact)";
    private const string InvalidTag = "ThisDoesNotExist";
    private const string CopyrightTag = "kantai_collection";
    private const string GeneralTag = "thighhighs";
    private const string MetadataTag = "highres";

    public class GetByName
    {
        [Test]
        [TestCase(typeof(Danbooru), ArtistTag, TagKind.Artist)]
        [TestCase(typeof(Danbooru), CharacterTag, TagKind.Character)]
        [TestCase(typeof(Danbooru), CopyrightTag, TagKind.Copyright)]
        [TestCase(typeof(Danbooru), GeneralTag, TagKind.General)]
        [TestCase(typeof(Danbooru), MetadataTag, TagKind.Metadata)]

        [TestCase(typeof(Gelbooru), ArtistTag, TagKind.Artist)]
        [TestCase(typeof(Gelbooru), CharacterTag, TagKind.Character)]
        [TestCase(typeof(Gelbooru), CopyrightTag, TagKind.Copyright)]
        [TestCase(typeof(Gelbooru), GeneralTag, TagKind.General)]
        [TestCase(typeof(Gelbooru), MetadataTag, TagKind.Metadata)]

        [TestCase(typeof(Konachan), ArtistTag, TagKind.Artist)]
        [TestCase(typeof(Konachan), CharacterTag, TagKind.Character)]
        [TestCase(typeof(Konachan), CopyrightTag, TagKind.Copyright)]
        [TestCase(typeof(Konachan), GeneralTag, TagKind.General)]
        [TestCase(typeof(Konachan), MetadataTag, TagKind.Metadata)]

        [TestCase(typeof(Yandere), ArtistTag, TagKind.Artist)]
        [TestCase(typeof(Yandere), CharacterTag, TagKind.Character)]
        [TestCase(typeof(Yandere), CopyrightTag, TagKind.Copyright)]
        [TestCase(typeof(Yandere), GeneralTag, TagKind.General)]
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
        public void GetByName_Cancellation(Type booruType, string name = CopyrightTag)
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
        public void GetByName_Fail(Type booruType, string name = InvalidTag)
        {
            var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

            Assert.ThrowsAsync<InvalidTagNameException>(() => booru.GetTagAsync(name));
        }
    }
}
