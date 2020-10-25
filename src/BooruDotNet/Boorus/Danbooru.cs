using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;

namespace BooruDotNet.Boorus
{
    public class Danbooru : BooruBase, IBooruPostsById
    {
        public Danbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruPost_Format, id);

            using Stream jsonStream = await GetResponseStreamAsync(uri);

            return await JsonSerializer.DeserializeAsync<DanbooruPost>(jsonStream);
        }
    }
}
