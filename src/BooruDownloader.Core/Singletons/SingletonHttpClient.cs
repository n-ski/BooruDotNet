using System;
using System.Net;
using System.Net.Http;
using BooruDotNet.Helpers;

namespace BooruDownloader.Singletons
{
    internal static class SingletonHttpClient
    {
        private static readonly Lazy<HttpClient> _instance = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            });

            // HACK: Band-aid fix for SankakuComplex downloads. I cba to create a separate downloader
            // for Sankaku posts.
            client.DefaultRequestHeaders.UserAgent.ParseAdd(NetHelper.UserAgentForRuntime);

            return client;
        });

        public static HttpClient Instance => _instance.Value;
    }
}
