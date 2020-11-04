using System;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Posts;
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
            var booru = BooruHelpers.Create<IBooruPostById>(booruType);

            var post = await booru.GetPostAsync(id);

            Assert.AreEqual(id, post.ID);
        }

        [Test]
        [TestCase(typeof(Danbooru), "a8044be47c86a36f7cf74253accd0752", 539253)]
        [TestCase(typeof(Gelbooru), "a8044be47c86a36f7cf74253accd0752", 608559)]
        public async Task GetByHash_Success(Type booruType, string hash, int expectedId)
        {
            var booru = BooruHelpers.Create<IBooruPostByHash>(booruType);

            var post = await booru.GetPostAsync(hash);

            Assert.AreEqual(expectedId, post.ID);
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public void GetById_Fail(Type booruType)
        {
            var booru = BooruHelpers.Create<IBooruPostById>(booruType);

            Assert.ThrowsAsync<InvalidPostIdException>(() => booru.GetPostAsync(0));
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        public void GetByHash_Fail(Type booruType)
        {
            var booru = BooruHelpers.Create<IBooruPostByHash>(booruType);
            var hash = new string('0', 32);

            Assert.ThrowsAsync<InvalidPostHashException>(() => booru.GetPostAsync(hash));
        }
    }
}