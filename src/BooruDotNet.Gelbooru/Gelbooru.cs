using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Extensions;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Boorus
{
    public class Gelbooru : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Gelbooru(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            return GetPostAsync(id, cancellationToken, true);
        }

        public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(hash, nameof(hash));

            Uri uri = UriHelper.CreateFormat(Uris.Gelbooru_PostHash_Format, hash);

            // Gelbooru doesn't respond with JSON directly, but it does
            // redirect us to the actual post.
            using HttpResponseMessage response = await HttpClient.HeadAsync(uri, cancellationToken).CAF();

            response.EnsureSuccessStatusCode();

            Uri? redirectUri = response.RequestMessage?.RequestUri;
            string? id;

            if (redirectUri is object)
            {
                // If redirect wasn't successful, then the redirect URI will stay the same.
                id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");
            }
            else
            {
                id = null;
            }

            Error.IfNot<InvalidPostHashException>(int.TryParse(id, out int postId), hash);

            try
            {
                return await GetPostAsync(postId, cancellationToken, false).CAF();
            }
            catch (JsonException)
            {
                throw new InvalidPostHashException(hash);
            }
            catch (InvalidPostIdException)
            {
                throw new InvalidPostHashException(hash);
            }
        }

        public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            string escapedName = Uri.EscapeDataString(tagName);

            Uri uri = UriHelper.CreateFormat(Uris.Gelbooru_TagName_Format, escapedName);

            var response = await HttpClient.GetFromJsonAsync<ApiResponse>(uri, cancellationToken).CAF();

            return response?.Tags?.Length is 1 ? response.Tags[0] : throw new InvalidTagNameException(tagName);
        }

        // Special method that can handle thrown JsonException.
        // Exception doesn't need to be handled if this method was called by GetPostAsync(string)
        private async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken, bool handleJsonException)
        {
            Uri uri = UriHelper.CreateFormat(Uris.Gelbooru_PostId_Format, id);

            GelbooruPost[]? posts;

            try
            {
                var response = await HttpClient.GetFromJsonAsync<ApiResponse>(uri, cancellationToken).CAF();
                posts = response?.Posts;
            }
            catch (JsonException) when (handleJsonException)
            {
                throw new InvalidPostIdException(id);
            }

            return posts?.Length is 1 ? posts[0] : throw new InvalidPostIdException(id);
        }
    }
}
