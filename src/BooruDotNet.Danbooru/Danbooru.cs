using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Validation;

#if NET5_0_OR_GREATER
using System.Net;
#endif

namespace BooruDotNet.Boorus;

public class Danbooru : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
{
    public Danbooru(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
    {
        Uri uri = UriHelper.CreateFormat(Uris.Danbooru_PostId_Format, id);

        IPost? post;

        try
        {
            post = await HttpClient.GetFromJsonAsync<DanbooruPost>(uri, cancellationToken).CAF();
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

    public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
    {
        Requires.NotNullOrWhiteSpace(hash, nameof(hash));

        Uri uri = UriHelper.CreateFormat(Uris.Danbooru_PostHash_Format, hash);

        IPost? post;

        try
        {
            post = await HttpClient.GetFromJsonAsync<DanbooruPost>(uri, cancellationToken).CAF();
        }
#if NET5_0_OR_GREATER
        catch (HttpRequestException exception) when (exception.StatusCode is HttpStatusCode.NotFound)
#else
        catch (HttpRequestException exception) when (exception.Message.Contains("404"))
#endif
        {
            post = null;
        }

        return post ?? throw new InvalidPostHashException(hash);
    }

    public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
    {
        Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

        string escapedName = Uri.EscapeDataString(tagName);

        Uri uri = UriHelper.CreateFormat(Uris.Danbooru_TagName_Format, escapedName);

        DanbooruTag[]? tags = await HttpClient.GetFromJsonAsync<DanbooruTag[]>(uri, cancellationToken).CAF();

        return tags?.Length is 1 ? tags[0] : throw new InvalidTagNameException(tagName);
    }
}
