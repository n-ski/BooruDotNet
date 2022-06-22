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

public class Konachan : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
{
    public Konachan(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
    {
        Uri uri = UriHelper.CreateFormat(Uris.Konachan_PostId_Format, id);

        KonachanPost[]? posts;

        try
        {
            posts = await HttpClient.GetFromJsonAsync<KonachanPost[]>(uri, cancellationToken).CAF();
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

        Uri uri = UriHelper.CreateFormat(Uris.Konachan_PostHash_Format, hash);
        KonachanPost[]? posts;

        try
        {
            posts = await HttpClient.GetFromJsonAsync<KonachanPost[]>(uri, cancellationToken).CAF();
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

        Uri uri = UriHelper.CreateFormat(Uris.Konachan_TagName_Format, escapedName);

        KonachanTag[]? tags = await HttpClient.GetFromJsonAsync<KonachanTag[]>(uri, cancellationToken).CAF();

        foreach (KonachanTag tag in tags!)
        {
            // Tag search is case-sensitive.
            if (tag.Name == tagName)
            {
                return tag;
            }
        }

        throw new InvalidTagNameException(tagName);
    }
}
