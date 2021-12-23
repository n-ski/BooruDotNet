using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Boorus
{
    public class Konachan : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Konachan(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            Uri uri = UriHelper.CreateFormat(Uris.Konachan_PostId_Format, id);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken, false).CAF();

            Error.If<InvalidPostIdException>(response.StatusCode == HttpStatusCode.NotFound, id);
            response.EnsureSuccessStatusCode();

            KonachanPost[] posts = await DeserializeAsync<KonachanPost[]>(response, cancellationToken).CAF();

            Error.IfNot<InvalidPostIdException>(posts.Length == 1, id);

            return posts[0];
        }

        public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(hash, nameof(hash));

            Uri uri = UriHelper.CreateFormat(Uris.Konachan_PostHash_Format, hash);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken).CAF();

            KonachanPost[] posts = await DeserializeAsync<KonachanPost[]>(response, cancellationToken).CAF();

            Error.IfNot<InvalidPostHashException>(posts.Length == 1, hash);

            return posts[0];
        }

        public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            string escapedName = Uri.EscapeDataString(tagName);

            Uri uri = UriHelper.CreateFormat(Uris.Konachan_TagName_Format, escapedName);

            KonachanTag[] tags = await GetResponseAndDeserializeAsync<KonachanTag[]>(uri, cancellationToken).CAF();

            foreach (KonachanTag tag in tags)
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
}
