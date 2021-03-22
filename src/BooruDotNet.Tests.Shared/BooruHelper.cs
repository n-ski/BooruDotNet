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
            SankakuComplex = new SankakuComplex(HttpClient);
            Yandere = new Yandere(HttpClient);

            PostCaches = new Dictionary<Type, PostCache>
            {
                [typeof(Danbooru)] = new PostCache(Danbooru),
                [typeof(Gelbooru)] = new PostCache(Gelbooru),
                [typeof(SankakuComplex)] = new PostCache(SankakuComplex),
                [typeof(Yandere)] = new PostCache(Yandere),
            }.ToImmutableDictionary();

            TagCaches = new Dictionary<Type, TagCache>
            {
                [typeof(Danbooru)] = new TagCache(Danbooru),
                [typeof(Gelbooru)] = new TagCache(Gelbooru),
                [typeof(Yandere)] = new TagCache(Yandere),
            }.ToImmutableDictionary();

            TaskCancellationDelay = TimeSpan.FromMilliseconds(50);
        }

        internal static HttpClient HttpClient { get; }

        internal static Danbooru Danbooru { get; }

        internal static Gelbooru Gelbooru { get; }

        internal static SankakuComplex SankakuComplex { get; }

        internal static Yandere Yandere { get; }

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
