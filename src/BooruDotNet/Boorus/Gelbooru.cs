using System;
using System.Net.Http;
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

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostId_Format, id);

            GelbooruPost[] posts = await GetResponseAndDeserializeAsync<GelbooruPost[]>(uri);

            Ensure.That<HttpRequestException>(
                posts.Length == 1,
                ErrorMessages.PostInvalidHash);

            return posts[0];
        }

        public async Task<IPost> GetPostAsync(string hash)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(hash);

            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostHash_Format, hash);

            // Gelbooru doesn't respond with JSON directly, but it does
            // redirect us to the actual post.
            HttpResponseMessage response = await HttpClient.HeadAsync(uri);
            response.EnsureSuccessStatusCode();

            Uri redirectUri = response.RequestMessage.RequestUri;
            string id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");

            Ensure.That<HttpRequestException>(
                int.TryParse(id, out int postId),
                ErrorMessages.PostInvalidHash);

            return await GetPostAsync(postId);
        }

        public async Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruTagName_Format, tagName);

            GelbooruTag[] tags = await GetResponseAndDeserializeAsync<GelbooruTag[]>(uri);

            Ensure.That<HttpRequestException>(
                tags.Length == 1,
                ErrorMessages.TagInvalidName);

            return tags[0];
        }
    }
}
