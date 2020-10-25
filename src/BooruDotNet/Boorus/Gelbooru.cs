using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using BooruDotNet.Extensions;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;

namespace BooruDotNet.Boorus
{
    public class Gelbooru : BooruBase, IBooruPostsById, IBooruPostsByHash
    {
        public Gelbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostId_Format, id);

            using Stream jsonStream = await GetResponseStreamAsync(uri);
            GelbooruPost[] posts = await JsonSerializer.DeserializeAsync<GelbooruPost[]>(jsonStream);

            return posts[0];
        }

        public async Task<IPost> GetPostAsync(string hash)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPostHash_Format, hash);

            // Gelbooru doesn't respond with JSON directly, but it does
            // redirect us to the actual post.
            HttpResponseMessage response = await HttpClient.HeadAsync(uri);
            response.EnsureSuccessStatusCode();

            Uri redirectUri = response.RequestMessage.RequestUri;
            string id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");

            if (!int.TryParse(id, out int postId))
            {
                throw new HttpRequestException();
            }

            return await GetPostAsync(postId);
        }
    }
}
