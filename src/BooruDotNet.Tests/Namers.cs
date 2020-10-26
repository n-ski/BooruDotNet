using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Namers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Namers
    {
        private static readonly Danbooru _booru = new Danbooru();
        private static readonly IPostNamer _hashNamer = new HashNamer();
        private static readonly IPostNamer _danbooruNamer = new DanbooruNamer(_booru);

        [Test]
        public async Task CreateHashName_Success()
        {
            var post = await _booru.GetPostAsync(123456);
            var expectedName = post.Hash ?? "";

            var actualName = _hashNamer.Name(post);

            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        [TestCase(4000206, "__original__6f48ffed50db8cba28890e8ea0106219")]
        [TestCase(4001525, "__4001525__1e2478198f9b217c51ea682ca54e88f6")]
        [TestCase(4048966, "__drawn_by_zuizi__fce55cfd3f0d13ed59795c843c7814e2")]
        [TestCase(4050260, "__link_the_legend_of_zelda_and_1_more_drawn_by_india_swift_and_michael_doig__7353e754b624cbfa4e034d3bf348a46b")]
        [TestCase(4056776, "__artoria_pendragon_saber_nero_claudius_and_nero_claudius_fate_and_1_more_drawn_by_ishii_wami5285__9b56d855dcbeb438829b1a68a26930b1")]
        [TestCase(4059916, "__nero_claudius_and_nero_claudius_fate_and_1_more_drawn_by_rikui_rella2930__1a09746471cb49c4196ea32135280e63")]
        [TestCase(4060349, "__shishiro_botan_hololive_drawn_by_gfpebs__ec38eb54f588ffcd101e25ad0895bffd")]
        [TestCase(4060861, "__shishiro_botan_yukihana_lamy_mano_aloe_momosuzu_nene_omaru_polka_and_1_more_hololive_drawn_by_misono_denpachi__002754f8d993c5bf7645a6735abc2307")]
        [TestCase(4061119, "__original_drawn_by_i_takashi__590cb03c4efec8d13e3fb42c32c38efc")]
        [TestCase(4067797, "__kurata_mashiro_futaba_tsukushi_yashio_rui_hiromachi_nanami_and_kirigaya_touko_bang_dream_drawn_by_ayasaka__bd4750e801cc84fd2dc1d81ecfd4a3b0")]
        [TestCase(4166623, "__paimon_and_lumine_genshin_impact_drawn_by_ichijiku_suisei__bbc70109203a07c913a2a0f8e418391e")]
        public async Task CreateDanbooruName_Success(int postId, string expectedName)
        {
            var post = await _booru.GetPostAsync(postId);

            var actualName = _danbooruNamer.Name(post);

            Assert.AreEqual(expectedName, actualName);
        }

    }
}
