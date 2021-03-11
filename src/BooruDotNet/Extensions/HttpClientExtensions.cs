using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Validation;

namespace BooruDotNet.Extensions
{
    internal static class HttpClientExtensions
    {
        internal static Task<HttpResponseMessage> HeadAsync(this HttpClient httpClient, Uri requestUri,
            CancellationToken cancellationToken = default)
        {
            Assumes.NotNull(httpClient);
            Assumes.NotNull(requestUri);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
            return httpClient.SendAsync(requestMessage, cancellationToken);
        }
    }
}
