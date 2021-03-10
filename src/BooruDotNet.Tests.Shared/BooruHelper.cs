using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using BooruDotNet.Boorus;
using BooruDotNet.Caching;

namespace BooruDotNet.Tests.Shared
{
    internal static class BooruHelper
    {
        static BooruHelper()
        {
            HttpClient = new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });

            Danbooru = new Danbooru(HttpClient);
            Gelbooru = new Gelbooru(HttpClient);

            PostCaches = new Dictionary<Type, PostCache>
            {
                [typeof(Danbooru)] = new PostCache(Danbooru),
                [typeof(Gelbooru)] = new PostCache(Gelbooru),
            }.ToImmutableDictionary();

            TagCaches = new Dictionary<Type, TagCache>
            {
                [typeof(Danbooru)] = new TagCache(Danbooru),
                [typeof(Gelbooru)] = new TagCache(Gelbooru),
            }.ToImmutableDictionary();

            TaskCancellationDelay = TimeSpan.FromMilliseconds(50);
        }

        internal static HttpClient HttpClient { get; }

        internal static Danbooru Danbooru { get; }

        internal static Gelbooru Gelbooru { get; }

        internal static IReadOnlyDictionary<Type, PostCache> PostCaches { get; } 

        internal static IReadOnlyDictionary<Type, TagCache> TagCaches { get; } 

        internal static TimeSpan TaskCancellationDelay { get; }

        internal static T CreateBooru<T>(Type type)
        {
            T booru = (T)Activator.CreateInstance(type, HttpClient)!;
            return booru;
        }
    }
}
