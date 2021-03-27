using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Danbooru.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Danbooru
{
    public class Danbooru : BooruBase, IBooruPostById, IBooruPostByHash, IBooruTagByName
    {
        public Danbooru(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public async Task<IPost> GetPostAsync(int id, CancellationToken cancellationToken = default)
        {
            Uri uri = UriHelper.CreateFormat(Uris.Danbooru_PostId_Format, id);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken, false)
                .ConfigureAwait(false);

            Error.If<InvalidPostIdException>(response.StatusCode == HttpStatusCode.NotFound, id);
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<DanbooruPost>(response, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(hash, nameof(hash));

            Uri uri = UriHelper.CreateFormat(Uris.Danbooru_PostHash_Format, hash);

            using HttpResponseMessage response = await GetResponseAsync(uri, cancellationToken, false)
                .ConfigureAwait(false);

            Error.If<InvalidPostHashException>(response.StatusCode == HttpStatusCode.NotFound, hash);
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<DanbooruPost>(response, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            Uri uri = UriHelper.CreateFormat(Uris.Danbooru_TagName_Format, tagName);

            DanbooruTag[] tags = await GetResponseAndDeserializeAsync<DanbooruTag[]>(uri, cancellationToken)
                .ConfigureAwait(false);

            Error.IfNot<InvalidTagNameException>(tags.Length == 1, tagName);

            return tags[0];
        }
    }
}
