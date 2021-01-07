using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Links;
using BooruDotNet.Tests.Helpers;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Resolvers
    {
        static Resolvers()
        {
            BooruHelpers.RegisterResolvers();
        }

        [Test, Order(0)]
        [TestCase("https://danbooru.donmai.us/posts/133337")]
        [TestCase("https://gelbooru.com/index.php?page=post&s=view&id=133337")]
        [TestCase("http://gelbooru.com/index.php?page=post&s=list&md5=1b192c9bcc1bfdb5362ac2387dad4d1e")]
        public void ResolveHash_Cancellation(string url)
        {
            using var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(BooruHelpers.TaskCancellationDelay);

            Assert.ThrowsAsync<TaskCanceledException>(() => LinkResolver.ResolveAsync(new Uri(url), tokenSource.Token));
        }

        [Test, Order(1)]
        [TestCase("https://danbooru.donmai.us/posts/133337", 133337)]
        [TestCase("https://gelbooru.com/index.php?page=post&s=view&id=133337", 133337)]
        [TestCase("http://gelbooru.com/index.php?page=post&s=list&md5=1b192c9bcc1bfdb5362ac2387dad4d1e", 5749999)]
        public async Task Resolve_Success(string url, int expectedId)
        {
            var resolved = await LinkResolver.ResolveAsync(new Uri(url));

            Assert.AreEqual(expectedId, resolved.ID);
        }
    }
}
