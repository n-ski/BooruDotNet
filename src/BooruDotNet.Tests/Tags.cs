using System;
using System.Net.Http;
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
            var booru = BooruHelpers.Create<IBooruTagByName>(booruType);

            var tag = await booru.GetTagAsync(name);

            Assert.AreEqual(name, tag.Name);
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public void GetByName_Fail(Type booruType)
        {
            var booru = BooruHelpers.Create<IBooruTagByName>(booruType);

            Assert.ThrowsAsync<HttpRequestException>(() => booru.GetTagAsync("ThisDoesNotExist"));
        }
    }
}
