using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Boorus
{
    public abstract class BooruBase
    {
        private static HttpClient? _customHttpClient;

        protected BooruBase()
        {
        }

        [AllowNull]
        public static HttpClient HttpClient
        {
            protected get => _customHttpClient ?? SingletonHttpClient.Instance;
            set => _customHttpClient = value;
        }

        protected async static Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request,
            CancellationToken cancellationToken, bool ensureSuccess = true)
        {
            HttpResponseMessage response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            return ensureSuccess ? response.EnsureSuccessStatusCode() : response;
        }

        protected async static Task<HttpResponseMessage> GetResponseAsync(Uri requestUri,
            CancellationToken cancellationToken, bool ensureSuccess = true)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            return ensureSuccess ? response.EnsureSuccessStatusCode() : response;
        }

        protected async static Task<T> GetResponseAndDeserializeAsync<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await GetResponseAsync(requestUri, cancellationToken);

            return await DeserializeAsync<T>(response, cancellationToken);
        }

        protected async static Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            using Stream jsonStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(jsonStream, cancellationToken: cancellationToken);
        }
    }
}
