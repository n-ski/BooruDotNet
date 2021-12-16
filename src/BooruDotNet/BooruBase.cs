using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Validation;

namespace BooruDotNet
{
    public abstract class BooruBase
    {
        protected BooruBase(HttpClient httpClient)
        {
            HttpClient = Requires.NotNull(httpClient, nameof(httpClient));
        }

        protected HttpClient HttpClient { get; }

        protected async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request,
            CancellationToken cancellationToken, bool ensureSuccess = true)
        {
            HttpResponseMessage response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).CAF();

            return ensureSuccess ? response.EnsureSuccessStatusCode() : response;
        }

        protected async Task<HttpResponseMessage> GetResponseAsync(Uri requestUri,
            CancellationToken cancellationToken, bool ensureSuccess = true)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).CAF();

            return ensureSuccess ? response.EnsureSuccessStatusCode() : response;
        }

        protected async Task<T> GetResponseAndDeserializeAsync<T>(Uri requestUri, CancellationToken cancellationToken,
            JsonSerializerOptions? options = null)
        {
            using HttpResponseMessage response = await GetResponseAsync(requestUri, cancellationToken).CAF();

            return await DeserializeAsync<T>(response, cancellationToken, options).CAF();
        }

        protected async static Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken,
            JsonSerializerOptions? options = null)
        {
            using Stream jsonStream = await response.Content.ReadAsStreamAsync().CAF();

            var deserialized = await JsonSerializer.DeserializeAsync<T>(
                jsonStream,
                options,
                cancellationToken).CAF();

            return deserialized!;
        }
    }
}
