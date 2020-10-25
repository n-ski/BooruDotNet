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
    }
}