using System.Threading.Tasks;
using BooruDotNet.Caching;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Namers.Tests
{
    public class Namers
    {
        private static readonly PostCache _postCache = BooruHelper.PostCaches[typeof(Danbooru.Danbooru)];
        private static readonly TagCache _tagCache = BooruHelper.TagCaches[typeof(Danbooru.Danbooru)];
        private static readonly IPostNamer _hashNamer = new HashNamer();
        private static readonly IPostNamer _danbooruNamer = new DanbooruNamer(_tagCache);
        private static readonly IPostNamer _fancyNamer = new DanbooruFancyNamer(_tagCache);

        [Test]
        public async Task CreateHashName_Success()
        {
            var post = await _postCache.GetPostAsync(123456);
            var expectedName = post.Hash ?? "";

            var actualName = _hashNamer.Name(post);

            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        [TestCase(4000206, "__original__6f48ffed50db8cba28890e8ea0106219")]
        [TestCase(4001525, "__4001525__1e2478198f9b217c51ea682ca54e88f6")]
        [TestCase(4031417, "__sakura_miko_hololive_drawn_by_hoshimachi_suisei_artist_houshou_marine_artist_shigure_ui_and_tsukudani_norio__74cf170be86a8ec7a0a0cc420f762219")]
        [TestCase(4048966, "__drawn_by_zuizi__fce55cfd3f0d13ed59795c843c7814e2")]
        [TestCase(4050260, "__link_the_legend_of_zelda_and_1_more_drawn_by_india_swift_and_michael_doig__7353e754b624cbfa4e034d3bf348a46b")]
        [TestCase(4056776, "__artoria_pendragon_saber_nero_claudius_and_nero_claudius_fate_and_1_more_drawn_by_ishii_wami5285__9b56d855dcbeb438829b1a68a26930b1")]
        [TestCase(4059916, "__nero_claudius_and_nero_claudius_fate_and_1_more_drawn_by_rikui_rella2930__1a09746471cb49c4196ea32135280e63")]
        [TestCase(4060349, "__shishiro_botan_hololive_drawn_by_gfpebs__ec38eb54f588ffcd101e25ad0895bffd")]
        [TestCase(4060861, "__shishiro_botan_yukihana_lamy_momosuzu_nene_omaru_polka_mano_aloe_and_1_more_hololive_drawn_by_misono_denpachi__002754f8d993c5bf7645a6735abc2307")]
        [TestCase(4061119, "__original_drawn_by_i_takashi__590cb03c4efec8d13e3fb42c32c38efc")]
        [TestCase(4067797, "__kurata_mashiro_futaba_tsukushi_yashio_rui_kirigaya_touko_and_hiromachi_nanami_bang_dream_drawn_by_ayasaka__bd4750e801cc84fd2dc1d81ecfd4a3b0")]
        [TestCase(4090626, "__vrchat_drawn_by_kirisame_mia__c903eb2db92678bba106c96b7e71a923")]
        [TestCase(4166623, "__lumine_and_paimon_genshin_impact_drawn_by_ichijiku_suisei__bbc70109203a07c913a2a0f8e418391e")]
        public async Task CreateDanbooruName_Success(int postId, string expectedName)
        {
            var post = await _postCache.GetPostAsync(postId);

            var actualName = _danbooruNamer.Name(post);

            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        [TestCase(4000206, "_original  - 6f48ffed50db8cba28890e8ea0106219")]
        [TestCase(4001525, "#4001525 - 1e2478198f9b217c51ea682ca54e88f6")]
        [TestCase(4031417, "sakura miko (hololive) drawn by hoshimachi_suisei_(artist), houshou_marine_(artist), shigure_ui, and tsukudani_norio - 74cf170be86a8ec7a0a0cc420f762219")]
        [TestCase(4048966, "_ drawn by zuizi - fce55cfd3f0d13ed59795c843c7814e2")]
        [TestCase(4050260, "link (the legend of zelda and 1 more) drawn by india_swift and michael_doig - 7353e754b624cbfa4e034d3bf348a46b")]
        [TestCase(4056776, "artoria pendragon, saber, nero claudius, and nero claudius (fate and 1 more) drawn by ishii_(wami5285) - 9b56d855dcbeb438829b1a68a26930b1")]
        [TestCase(4059916, "nero claudius and nero claudius (fate and 1 more) drawn by rikui_(rella2930) - 1a09746471cb49c4196ea32135280e63")]
        [TestCase(4060349, "shishiro botan (hololive) drawn by gfpebs - ec38eb54f588ffcd101e25ad0895bffd")]
        [TestCase(4060861, "shishiro botan, yukihana lamy, momosuzu nene, omaru polka, mano aloe, and 1 more (hololive) drawn by misono_denpachi - 002754f8d993c5bf7645a6735abc2307")]
        [TestCase(4061119, "_original drawn by i.takashi - 590cb03c4efec8d13e3fb42c32c38efc")]
        [TestCase(4067797, "kurata mashiro, futaba tsukushi, yashio rui, kirigaya touko, and hiromachi nanami (bang dream!) drawn by ayasaka - bd4750e801cc84fd2dc1d81ecfd4a3b0")]
        [TestCase(4090626, "_vrchat drawn by kirisame_mia - c903eb2db92678bba106c96b7e71a923")]
        [TestCase(4166623, "lumine and paimon (genshin impact) drawn by ichijiku_suisei - bbc70109203a07c913a2a0f8e418391e")]
        // TODO: case with multiple copyrights but no characters.
        // TODO: case with a character but no copyrights.
        public async Task CreateFancyName_Success(int postId, string expectedName)
        {
            var post = await _postCache.GetPostAsync(postId);

            var actualName = _fancyNamer.Name(post);

            Assert.AreEqual(expectedName, actualName);
        }
    }
}
