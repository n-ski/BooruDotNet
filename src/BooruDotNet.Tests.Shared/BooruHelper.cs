using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
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

            Danbooru = new Danbooru.Danbooru(HttpClient);
            Gelbooru = new Gelbooru.Gelbooru(HttpClient);
            SankakuComplex = new SankakuComplex.SankakuComplex(HttpClient);
            Yandere = new Yandere.Yandere(HttpClient);

            PostCaches = new Dictionary<Type, PostCache>
            {
                [typeof(Danbooru.Danbooru)] = new PostCache(Danbooru),
                [typeof(Gelbooru.Gelbooru)] = new PostCache(Gelbooru),
                [typeof(SankakuComplex.SankakuComplex)] = new PostCache(SankakuComplex),
                [typeof(Yandere.Yandere)] = new PostCache(Yandere),
            }.ToImmutableDictionary();

            TagCaches = new Dictionary<Type, TagCache>
            {
                [typeof(Danbooru.Danbooru)] = new TagCache(Danbooru),
                [typeof(Gelbooru.Gelbooru)] = new TagCache(Gelbooru),
                [typeof(Yandere.Yandere)] = new TagCache(Yandere),
            }.ToImmutableDictionary();

            TaskCancellationDelay = TimeSpan.FromMilliseconds(50);
        }

        internal static HttpClient HttpClient { get; }

        internal static Danbooru.Danbooru Danbooru { get; }

        internal static Gelbooru.Gelbooru Gelbooru { get; }

        internal static SankakuComplex.SankakuComplex SankakuComplex { get; }

        internal static Yandere.Yandere Yandere { get; }

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
