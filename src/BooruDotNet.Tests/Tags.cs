﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Tags;
using BooruDotNet.Tests.Shared;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Tags
    {
        [Test]
        [TestCase(typeof(Danbooru.Danbooru))]
        [TestCase(typeof(Gelbooru.Gelbooru))]
        [TestCase(typeof(Konachan.Konachan))]
        [TestCase(typeof(Yandere.Yandere))]
        public async Task GetByName_Success(Type booruType, string name = "pantyhose")
        {
            var booru = BooruHelper.TagCaches[booruType];

            var tag = await booru.GetTagAsync(name);

            Assert.AreEqual(name, tag.Name);
        }

        [Test]
        [TestCase(typeof(Danbooru.Danbooru))]
        [TestCase(typeof(Gelbooru.Gelbooru))]
        [TestCase(typeof(Konachan.Konachan))]
        [TestCase(typeof(Yandere.Yandere))]
        public void GetByName_Cancellation(Type booruType, string name = "pantyhose")
        {
            // IMPORTANT: create raw instance here to not mess with other tests.
            // See TagsCache.cs.
            var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(BooruHelper.TaskCancellationDelay);

            Assert.ThrowsAsync<TaskCanceledException>(() => booru.GetTagAsync(name, tokenSource.Token));
        }

        [Test]
        [TestCase(typeof(Danbooru.Danbooru))]
        [TestCase(typeof(Gelbooru.Gelbooru))]
        [TestCase(typeof(Konachan.Konachan))]
        [TestCase(typeof(Konachan.Konachan), "Pantyhose")] // Case-sensitive search.
        [TestCase(typeof(Yandere.Yandere))]
        public void GetByName_Fail(Type booruType, string name = "ThisDoesNotExist")
        {
            var booru = BooruHelper.CreateBooru<IBooruTagByName>(booruType);

            Assert.ThrowsAsync<InvalidTagNameException>(() => booru.GetTagAsync(name));
        }
    }
}
