using System;
using System.Net;
using System.Net.Http;
using BooruDotNet.Boorus;

namespace BooruDotNet.Tests;

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
        Konachan = new Konachan(HttpClient);
        SankakuComplex = new SankakuComplex(HttpClient);
        Yandere = new Yandere(HttpClient);

        TaskCancellationDelay = TimeSpan.FromMilliseconds(50);
    }

    internal static HttpClient HttpClient { get; }

    internal static Danbooru Danbooru { get; }

    internal static Gelbooru Gelbooru { get; }

    internal static Konachan Konachan { get; }

    internal static SankakuComplex SankakuComplex { get; }

    internal static Yandere Yandere { get; }

    internal static TimeSpan TaskCancellationDelay { get; }

    internal static T CreateBooru<T>(Type type)
    {
        T booru = (T)Activator.CreateInstance(type, HttpClient)!;
        return booru;
    }
}
