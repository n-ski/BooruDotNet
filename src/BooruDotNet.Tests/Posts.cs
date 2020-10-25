using System;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Tests.Helpers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Posts
    {
        [Test]
        [TestCase(typeof(Danbooru), 123456)]
        [TestCase(typeof(Gelbooru), 5632370)]
        public async Task GetById_Success(Type booruType, int id)
        {
            var booru = BooruHelpers.Create<IBooruPostsById>(booruType);

            var post = await booru.GetPostAsync(id);

            Assert.AreEqual(id, post.ID);
        }

        [Test]
        [TestCase(typeof(Danbooru), "a8044be47c86a36f7cf74253accd0752", 539253)]
        [TestCase(typeof(Gelbooru), "a8044be47c86a36f7cf74253accd0752", 608559)]
        public async Task GetByHash_Success(Type booruType, string hash, int expectedId)
        {
            var booru = BooruHelpers.Create<IBooruPostsByHash>(booruType);

            var post = await booru.GetPostAsync(hash);

            Assert.AreEqual(expectedId, post.ID);
        }
    }
}