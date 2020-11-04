using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BooruDotNet.Boorus
{
    public abstract class BooruBase
    {
        private static readonly Lazy<HttpClient> _globalHttpClientLazy = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = 5,
            });

            var assemblyName = typeof(BooruBase).Assembly.GetName();

            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    assemblyName.Name!,
                    assemblyName.Version!.ToString(3)));

            return client;
        });
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

        protected async static Task<HttpResponseMessage> GetResponseAsync(Uri requestUri, bool ensureSuccess = true)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead);

            return ensureSuccess ? response.EnsureSuccessStatusCode() : response;
        }

        protected async static Task<T> GetResponseAndDeserializeAsync<T>(Uri requestUri)
        {
            HttpResponseMessage response = await GetResponseAsync(requestUri);

            return await DeserializeAsync<T>(response);
        }

        protected async static Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            using Stream jsonStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(jsonStream);
        }
    }
}
