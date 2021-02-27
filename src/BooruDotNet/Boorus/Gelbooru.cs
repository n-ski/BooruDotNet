using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BooruDotNet.Extensions;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Boorus
{
    public class Gelbooru : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Gelbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostId_Format, id);

            GelbooruPost[] posts = await GetResponseAndDeserializeAsync<GelbooruPost[]>(uri, cancellationToken)
                .ConfigureAwait(false);

            Error.IfNot<InvalidPostIdException>(posts.Length == 1, id);

            return posts[0];
        }

        public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(hash);

            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostHash_Format, hash);

            // Gelbooru doesn't respond with JSON directly, but it does
            // redirect us to the actual post.
            using HttpResponseMessage response = await HttpClient.HeadAsync(uri, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            Uri redirectUri = response.RequestMessage.RequestUri;
            // If redirect wasn't successful, then the redirect URI will stay the same.
            string id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");

            Error.IfNot<InvalidPostHashException>(int.TryParse(id, out int postId), hash);

            return await GetPostAsync(postId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruTagName_Format, tagName);

            GelbooruTag[] tags = await GetResponseAndDeserializeAsync<GelbooruTag[]>(uri, cancellationToken)
                .ConfigureAwait(false);

            Error.IfNot<InvalidTagNameException>(tags.Length == 1, tagName);

            return tags[0];
        }
    }
}
