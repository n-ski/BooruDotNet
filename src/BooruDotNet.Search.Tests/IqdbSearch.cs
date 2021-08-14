using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooruDotNet.Search.Extensions;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using BooruDotNet.Search.Tests.Helpers;
using NUnit.Framework;

namespace BooruDotNet.Search.Tests
{
    public class IqdbSearch
    {
        private static readonly Uri _testUri = new Uri("https://cdn.donmai.us/preview/47/fa/47faa37362c3eca37fb9cd7dab3545b8.jpg");

        public IqdbService Service => ServiceHelper.CreateService<IqdbService>();

        [Test]
        [TestCase(IqdbSearchOptions.SearchAnimePictures)]
        [TestCase(IqdbSearchOptions.SearchDanbooru)]
        [TestCase(IqdbSearchOptions.SearchEShuushuu)]
        [TestCase(IqdbSearchOptions.SearchGelbooru)]
        [TestCase(IqdbSearchOptions.SearchKonachan)]
        [TestCase(IqdbSearchOptions.SearchSankakuChannel)]
        [TestCase(IqdbSearchOptions.SearchYandere)]
        [TestCase(IqdbSearchOptions.SearchZerochan)]
        [TestCase(IqdbSearchOptions.SearchDanbooru | IqdbSearchOptions.SearchGelbooru)]
        public async Task SearchSelectedBoards_Success(IqdbSearchOptions value)
        {
            var resultsTask = Service.SearchAsync(_testUri, value);

            string[] hostNames = GetHostNamesFromEnum(value).ToArray();

            var results = await resultsTask;

            Assert.IsNotEmpty(results);

            foreach (IResult result in results)
            {
                Assert.Contains(result.Source.Host, hostNames);
            }
        }

        private static string GetHostNameFromEnum(IqdbSearchOptions value)
        {
            return value switch
            {
                IqdbSearchOptions.SearchAnimePictures => "anime-pictures.net",
                IqdbSearchOptions.SearchDanbooru => "danbooru.donmai.us",
                IqdbSearchOptions.SearchEShuushuu => "e-shuushuu.net",
                IqdbSearchOptions.SearchGelbooru => "gelbooru.com",
                IqdbSearchOptions.SearchKonachan => "konachan.com",
                IqdbSearchOptions.SearchSankakuChannel => "chan.sankakucomplex.com",
                IqdbSearchOptions.SearchYandere => "yande.re",
                IqdbSearchOptions.SearchZerochan => "www.zerochan.net",
                _ => throw new InvalidOperationException("Only one flag must be set."),
            };
        }

        private static IEnumerable<string> GetHostNamesFromEnum(IqdbSearchOptions value)
        {
            var allFlags = (IqdbSearchOptions[])Enum.GetValues(typeof(IqdbSearchOptions));

            foreach (IqdbSearchOptions flag in allFlags)
            {
                if (value.HasFlag(flag) && flag.TryGetServiceId(out _))
                {
                    yield return GetHostNameFromEnum(flag);
                }
            }
        }
    }
}
