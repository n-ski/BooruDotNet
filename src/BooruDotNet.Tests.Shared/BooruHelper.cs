using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using BooruDotNet.Boorus;
using BooruDotNet.Caching;

namespace BooruDotNet.Tests.Shared
{
    internal static class BooruHelper
    {
        internal static TimeSpan TaskCancellationDelay { get; } = TimeSpan.FromMilliseconds(50);

        internal static IReadOnlyDictionary<Type, PostCache> PostCaches { get; } =
            new Dictionary<Type, PostCache>
            {
                [typeof(Danbooru)] = new PostCache(Boorus.Danbooru),
                [typeof(Gelbooru)] = new PostCache(Boorus.Gelbooru),
            }.ToImmutableDictionary();

        internal static IReadOnlyDictionary<Type, TagCache> TagCaches { get; } =
            new Dictionary<Type, TagCache>
            {
                [typeof(Danbooru)] = new TagCache(Boorus.Danbooru),
                [typeof(Gelbooru)] = new TagCache(Boorus.Gelbooru),
            }.ToImmutableDictionary();

        internal static T Create<T>(Type type)
        {
            T booru = (T)Activator.CreateInstance(type)!;
            return booru;
        }

        internal static HttpClient HttpClient => SingletonHttpClient.Instance;
    }
}
