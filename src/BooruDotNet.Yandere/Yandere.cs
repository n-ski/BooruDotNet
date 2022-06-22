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

public class Yandere : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
{
    public Yandere(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
    {
        Uri uri = UriHelper.CreateFormat(Uris.Yandere_PostId_Format, id);

        YanderePost[]? posts;

        try
        {
            posts = await HttpClient.GetFromJsonAsync<YanderePost[]>(uri, cancellationToken).CAF();
        }
#if NET5_0_OR_GREATER
        catch (HttpRequestException exception) when (exception.StatusCode is HttpStatusCode.NotFound)
#else
        catch (HttpRequestException exception) when (exception.Message.Contains("404"))
#endif
        {
            posts = null;
        }

        return posts?.Length is 1 ? posts[0] : throw new InvalidPostIdException(id);
    }

    public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
    {
        Requires.NotNullOrWhiteSpace(hash, nameof(hash));

        Uri uri = UriHelper.CreateFormat(Uris.Yandere_PostHash_Format, hash);

        YanderePost[]? posts;

        try
        {
            posts = await HttpClient.GetFromJsonAsync<YanderePost[]>(uri, cancellationToken).CAF();
        }
#if NET5_0_OR_GREATER
        catch (HttpRequestException exception) when (exception.StatusCode is HttpStatusCode.NotFound)
#else
        catch (HttpRequestException exception) when (exception.Message.Contains("404"))
#endif
        {
            posts = null;
        }

        return posts?.Length is 1 ? posts[0] : throw new InvalidPostHashException(hash);
    }

    public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
    {
        Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

        string escapedName = Uri.EscapeDataString(tagName);

        Uri uri = UriHelper.CreateFormat(Uris.Yandere_TagName_Format, escapedName);

        YandereTag[]? tags = await HttpClient.GetFromJsonAsync<YandereTag[]>(uri, cancellationToken).CAF();

        foreach (YandereTag tag in tags!)
        {
            // Tag search is case-sensitive, see unit tests.
            if (tag.Name == tagName)
            {
                return tag;
            }
        }

        throw new InvalidTagNameException(tagName);
    }
}
