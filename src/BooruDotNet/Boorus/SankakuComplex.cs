using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;

namespace BooruDotNet.Boorus
{
    // Note: Sankaku Complex requires user agent, even if it contains some garbage data,
    // otherwise response will contain 500 error.
    public class SankakuComplex : BooruBase, IBooruPostById
    {
        public SankakuComplex(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            Uri uri = UriHelper.CreateFormat(RequestUris.SankakuComplex_PostId_Format, id);

            using HttpRequestMessage request = PrepareRequest(HttpMethod.Get, uri);
            using HttpResponseMessage response = await GetResponseAsync(request, cancellationToken, false)
                .ConfigureAwait(false);

            Error.If<InvalidPostIdException>(response.StatusCode == HttpStatusCode.NotFound, id);
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<SankakuComplexPost>(response, cancellationToken)
                .ConfigureAwait(false);
        }

        private HttpRequestMessage PrepareRequest(HttpMethod httpMethod, Uri uri)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            request.Headers.UserAgent.ParseAdd(NetHelper.UserAgentForRuntime);

            return request;
        }
    }
}
