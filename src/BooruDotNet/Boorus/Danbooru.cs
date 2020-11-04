using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Boorus
{
    public class Danbooru : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Danbooru() : base()
        {
        }

        public async Task<IPost> GetPostAsync(int id)
        {
            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruPostId_Format, id);

            HttpResponseMessage response = await GetResponseAsync(uri, false);

            Error.If<InvalidPostIdException>(response.StatusCode == HttpStatusCode.NotFound, id);
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<DanbooruPost>(response);
        }

        public async Task<IPost> GetPostAsync(string hash)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(hash);

            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruPostHash_Format, hash);

            HttpResponseMessage response = await GetResponseAsync(uri, false);

            Error.If<InvalidPostHashException>(response.StatusCode == HttpStatusCode.NotFound, hash);
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<DanbooruPost>(response);
        }

        public async Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            Uri uri = UriHelpers.CreateFormat(RequestUris.DanbooruTagName_Format, tagName);

            DanbooruTag[] tags = await GetResponseAndDeserializeAsync<DanbooruTag[]>(uri);

            Error.IfNot<InvalidTagNameException>(tags.Length == 1, tagName);

            return tags[0];
        }
    }
}
