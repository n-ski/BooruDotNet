using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruDotNet.Boorus
{
    public abstract class BooruBase
    {
        private static readonly Lazy<HttpClient> _globalHttpClientLazy = new Lazy<HttpClient>(
            () => new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            }));
        private static HttpClient? _customHttpClient;

        protected BooruBase()
        {
        }

        [AllowNull]
        public static HttpClient HttpClient
        {
            protected get => _customHttpClient ?? _globalHttpClientLazy.Value;
            set => _customHttpClient = value;
        }

        protected async static Task<HttpResponseMessage> GetResponseAsync(Uri requestUri)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead);

            return response.EnsureSuccessStatusCode();
        }

        protected async static Task<Stream> GetResponseStreamAsync(Uri requestUri)
        {
            HttpResponseMessage response = await GetResponseAsync(requestUri);

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
