using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using NUnit.Framework;

namespace BooruDotNet.Tests
{
    public class Search
    {
        [Test]
        public async Task SearchByUri_Success()
        {
            var service = new DanbooruService();

            var results = await service.SearchBy(new Uri("https://cdn.donmai.us/preview/47/fa/47faa37362c3eca37fb9cd7dab3545b8.jpg"));
            var firstResult = results.First();

            Assert.IsTrue(firstResult.Post.ID == 123456);
        }

        [Test]
        public async Task SearchByFile_Success()
        {
            var service = new DanbooruService();

            using var file = File.OpenRead(@".\Images\47faa37362c3eca37fb9cd7dab3545b8.jpg");
            var results = await service.SearchBy(file);
            var firstResult = results.First();

            Assert.IsTrue(firstResult.Post.ID == 123456);
        }
    }
}
