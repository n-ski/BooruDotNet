using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;

#if NET5_0_OR_GREATER
using System.Net;
#endif

namespace BooruDotNet.Boorus;

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
        Uri uri = UriHelper.CreateFormat(Uris.SankakuComplex_PostId_Format, id);

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd(NetHelper.UserAgentForRuntime);

        IPost? post;

        try
        {
            using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            post = await response.Content.ReadFromJsonAsync<SankakuComplexPost>(cancellationToken: cancellationToken).CAF();
        }
#if NET5_0_OR_GREATER
        catch (HttpRequestException exception) when (exception.StatusCode is HttpStatusCode.NotFound)
#else
        catch (HttpRequestException exception) when (exception.Message.Contains("404"))
#endif
        {
            post = null;
        }

        return post ?? throw new InvalidPostIdException(id);
    }
}
