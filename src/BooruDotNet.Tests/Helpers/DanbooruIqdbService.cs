using BooruDotNet.Search.Services;

namespace BooruDotNet.Tests.Helpers
{
    internal sealed class DanbooruIqdbService : IqdbService
    {
        public DanbooruIqdbService() : base("danbooru")
        {
        }
    }
}
