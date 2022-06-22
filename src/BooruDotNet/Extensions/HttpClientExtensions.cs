using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Extensions;

internal static class HttpClientExtensions
{
    internal static Task<HttpResponseMessage> HeadAsync(this HttpClient httpClient, Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        Debug.Assert(httpClient is object);
        Debug.Assert(requestUri is object);

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
        return httpClient.SendAsync(requestMessage, cancellationToken);
    }
}
