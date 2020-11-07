using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BooruDotNet.Boorus;
using BooruDotNet.Caches;

namespace BooruDotNet.Tests.Helpers
{
    internal static class BooruHelpers
    {
        private static readonly Danbooru _danbooru = new Danbooru();
        private static readonly Gelbooru _gelbooru = new Gelbooru();

        internal static IReadOnlyDictionary<Type, PostCache> PostCaches { get; } =
            new Dictionary<Type, PostCache>
            {
                [typeof(Danbooru)] = new PostCache(_danbooru),
                [typeof(Gelbooru)] = new PostCache(_gelbooru),
            }.ToImmutableDictionary();

        internal static IReadOnlyDictionary<Type, TagCache> TagCaches { get; } =
            new Dictionary<Type, TagCache>
            {
                [typeof(Danbooru)] = new TagCache(_danbooru),
                [typeof(Gelbooru)] = new TagCache(_gelbooru),
            }.ToImmutableDictionary();

        internal static T Create<T>(Type type)
        {
            T booru = (T)Activator.CreateInstance(type);
            return booru;
        }
    }
}
