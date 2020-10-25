using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using Easy.Common;

namespace BooruDotNet.Boorus
{
    public class Danbooru : BooruBase, IBooruPostsById, IBooruPostsByHash
    {
        public Danbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruPostId_Format, id);

            using Stream jsonStream = await GetResponseStreamAsync(uri);

            return await JsonSerializer.DeserializeAsync<DanbooruPost>(jsonStream);
        }

        public async Task<IPost> GetPostAsync(string hash)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(hash);

            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruPostHash_Format, hash);

            using Stream jsonStream = await GetResponseStreamAsync(uri);

            return await JsonSerializer.DeserializeAsync<DanbooruPost>(jsonStream);
        }
    }
}
