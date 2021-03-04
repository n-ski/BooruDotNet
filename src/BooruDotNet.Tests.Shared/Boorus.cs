using BooruDotNet.Boorus;

namespace BooruDotNet.Tests.Shared
{
    internal static class Boorus
    {
        internal static Danbooru Danbooru { get; } = new Danbooru();
        internal static Gelbooru Gelbooru { get; } = new Gelbooru();
    }
}
