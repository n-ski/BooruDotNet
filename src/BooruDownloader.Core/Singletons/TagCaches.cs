using System;
using BooruDotNet.Caching;

namespace BooruDownloader.Singletons
{
    internal static class TagCaches
    {
        private static readonly Lazy<TagCache> _danbooruCache = new Lazy<TagCache>(() => new TagCache(Boorus.Danbooru));

        public static TagCache DanbooruTagCache => _danbooruCache.Value;
    }
}
