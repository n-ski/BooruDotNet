using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Resources;
using BooruDotNet.Search.Resources;
using BooruDotNet.Search.Results;
using Easy.Common;

namespace BooruDotNet.Search.Services
{
    public class DanbooruService : ServiceBase, ISearchByUri
    {
        public DanbooruService()
            : base(HttpMethod.Get, new Uri(UploadUris.Danbooru))
        {
        }

        public async Task<IEnumerable<IResult>> SearchBy(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.That(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            using HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["url"] = uri.AbsoluteUri,
            });

            using Stream responseStream = await UploadContent(content, cancellationToken);

            return await JsonSerializer.DeserializeAsync<DanbooruResult[]>(responseStream);
        }
    }
}
