using System.Net.Http;
using BooruDotNet.Search.Services;

namespace BooruDotNet.Search.Tests
{
    public sealed class DanbooruIqdbService : IqdbService
    {
        public DanbooruIqdbService(HttpClient httpClient)
            : base(httpClient, "danbooru")
        {
        }
    }
}
