using System;
using BooruDotNet.Boorus;

namespace BooruDownloader.Singletons
{
    internal static class Boorus
    {
        private static readonly Lazy<Danbooru> _danbooru = new Lazy<Danbooru>(() => new Danbooru(SingletonHttpClient.Instance));
        private static readonly Lazy<Gelbooru> _gelbooru = new Lazy<Gelbooru>(() => new Gelbooru(SingletonHttpClient.Instance));
        private static readonly Lazy<Konachan> _konachan = new Lazy<Konachan>(() => new Konachan(SingletonHttpClient.Instance));
        private static readonly Lazy<SankakuComplex> _sankaku = new Lazy<SankakuComplex>(() => new SankakuComplex(SingletonHttpClient.Instance));
        private static readonly Lazy<Yandere> _yandere = new Lazy<Yandere>(() => new Yandere(SingletonHttpClient.Instance));

        public static Danbooru Danbooru => _danbooru.Value;
        public static Gelbooru Gelbooru => _gelbooru.Value;
        public static Konachan Konachan => _konachan.Value;
        public static SankakuComplex SankakuComplex => _sankaku.Value;
        public static Yandere Yandere => _yandere.Value;
    }
}
