using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using BooruDotNet.Yandere.Resources;
using Validation;

namespace BooruDotNet.Yandere
{
    public class Yandere : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Yandere(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            Uri uri = UriHelper.CreateFormat(Uris.Yandere_PostId_Format, id);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken, false)
                .ConfigureAwait(false);

            Error.If<InvalidPostIdException>(response.StatusCode == HttpStatusCode.NotFound, id);
            response.EnsureSuccessStatusCode();

            YanderePost[] posts = await DeserializeAsync<YanderePost[]>(response, cancellationToken)
                .ConfigureAwait(false);

            Error.IfNot<InvalidPostIdException>(posts.Length == 1, id);

            return posts[0];
        }

        public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(hash, nameof(hash));

            Uri uri = UriHelper.CreateFormat(Uris.Yandere_PostHash_Format, hash);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken)
                .ConfigureAwait(false);

            YanderePost[] posts = await DeserializeAsync<YanderePost[]>(response, cancellationToken)
                .ConfigureAwait(false);

            Error.IfNot<InvalidPostHashException>(posts.Length == 1, hash);

            return posts[0];
        }

        public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            Uri uri = UriHelper.CreateFormat(Uris.Yandere_TagName_Format, tagName);

            YandereTag[] tags = await GetResponseAndDeserializeAsync<YandereTag[]>(uri, cancellationToken)
                .ConfigureAwait(false);

            foreach (YandereTag tag in tags)
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
}
