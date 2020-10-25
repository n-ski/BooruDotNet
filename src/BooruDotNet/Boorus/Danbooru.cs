using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Boorus
{
    public class Danbooru : BooruBase, IBooruPostsById, IBooruPostsByHash, IBooruTagByName
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

            return await GetResponseAndDeserializeAsync<DanbooruPost>(uri);
        }

        public async Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruTagName_Format, tagName);

            DanbooruTag[] tags = await GetResponseAndDeserializeAsync<DanbooruTag[]>(uri);

            Ensure.That<HttpRequestException>(
                tags.Length == 1,
                ErrorMessages.TagInvalidName);

            return tags[0];
        }
    }
}
