using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;

namespace BooruDotNet.Boorus
{
    public class Gelbooru : BooruBase, IBooruPostsById
    {
        public Gelbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.GelbooruPost_Format, id);

            using Stream jsonStream = await GetResponseStreamAsync(uri);
            GelbooruPost[] posts = await JsonSerializer.DeserializeAsync<GelbooruPost[]>(jsonStream);

            return posts[0];
        }
    }
}
