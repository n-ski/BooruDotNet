using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Posts;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Posts
    {
        [Test]
        [TestCase(typeof(Danbooru), 123456)]
        [TestCase(typeof(Gelbooru), 5632370)]
        [TestCase(typeof(Konachan), 75857)]
        [TestCase(typeof(SankakuComplex), 5235625)]
        [TestCase(typeof(Yandere), 759835)]
        public async Task GetById_Success(Type booruType, int id)
        {
            var booru = BooruHelper.PostCaches[booruType];

            var post = await booru.GetPostAsync(id);

            Assert.AreEqual(id, post.ID);
        }

        [Test]
        [TestCase(typeof(Danbooru), "a8044be47c86a36f7cf74253accd0752", 539253)]
        [TestCase(typeof(Gelbooru), "a8044be47c86a36f7cf74253accd0752", 608559)]
        [TestCase(typeof(Konachan), "86818eedf6ddca7a42f88d7f240be1dc", 75857)]
        [TestCase(typeof(Yandere), "d483d8dfdc9753125d82b4561752d128", 759835)]
        public async Task GetByHash_Success(Type booruType, string hash, int expectedId)
        {
            var booru = BooruHelper.CreateBooru<IBooruPostByHash>(booruType);

            var post = await booru.GetPostAsync(hash);

            Assert.AreEqual(expectedId, post.ID);
        }

        [Test]
        [TestCase(typeof(Danbooru), 123456)]
        [TestCase(typeof(Gelbooru), 5632370)]
        [TestCase(typeof(Konachan), 75857)]
        [TestCase(typeof(SankakuComplex), 5235625)]
        [TestCase(typeof(Yandere), 759835)]
        public void GetById_Cancellation(Type booruType, int id)
        {
            // IMPORTANT: create raw instance here to not mess with other tests.
            // See PostCache.cs.
            var booru = BooruHelper.CreateBooru<IBooruPostById>(booruType);

            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

            Assert.ThrowsAsync<TaskCanceledException>(() => booru.GetPostAsync(id, tokenSource.Token));
        }

        [Test]
        [TestCase(typeof(Danbooru), "a8044be47c86a36f7cf74253accd0752")]
        [TestCase(typeof(Gelbooru), "a8044be47c86a36f7cf74253accd0752")]
        [TestCase(typeof(Konachan), "86818eedf6ddca7a42f88d7f240be1dc")]
        [TestCase(typeof(Yandere), "d483d8dfdc9753125d82b4561752d128")]
        public void GetByHash_Cancellation(Type booruType, string hash)
        {
            var booru = BooruHelper.CreateBooru<IBooruPostByHash>(booruType);

            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

            Assert.ThrowsAsync<TaskCanceledException>(() => booru.GetPostAsync(hash, tokenSource.Token));
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        [TestCase(typeof(Konachan))]
        [TestCase(typeof(SankakuComplex))]
        [TestCase(typeof(Yandere))]
        public void GetById_Fail(Type booruType)
        {
            var booru = BooruHelper.PostCaches[booruType];

            Assert.ThrowsAsync<InvalidPostIdException>(() => booru.GetPostAsync(0));
        }

        [Test]
        [TestCase(typeof(Danbooru))]
        [TestCase(typeof(Gelbooru))]
        [TestCase(typeof(Konachan))]
        [TestCase(typeof(Gelbooru), "7225a1f1acd195823053613e41953cd0")] // Deleted post.
        [TestCase(typeof(Yandere))]
        public void GetByHash_Fail(Type booruType, string? hash = null)
        {
            var booru = BooruHelper.CreateBooru<IBooruPostByHash>(booruType);
            hash ??= new string('0', 32);

            Assert.ThrowsAsync<InvalidPostHashException>(() => booru.GetPostAsync(hash));
        }
    }
}