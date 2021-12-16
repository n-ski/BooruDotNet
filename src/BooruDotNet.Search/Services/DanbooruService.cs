using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Extensions;
using BooruDotNet.Search.Resources;
using BooruDotNet.Search.Results;
using Validation;

namespace BooruDotNet.Search.Services
{
    public class DanbooruService : ServiceBase, IFileAndUriSearchService
    {
        public DanbooruService(HttpClient httpClient)
            : base(httpClient, HttpMethod.Get, new Uri(UploadUris.Danbooru))
        {
        }

        public long FileSizeLimit => long.MaxValue;

        public async Task<IEnumerable<IResult>> SearchAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            using HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["url"] = uri.AbsoluteUri,
            }!);

            return await UploadAndDeserializeAsync(content, cancellationToken).CAF();
        }

        public async Task<IEnumerable<IResult>> SearchAsync(
            FileStream fileStream, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(fileStream, nameof(fileStream));

            // TODO: check file size.

            using HttpContent content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "search[file]", Path.GetFileName(fileStream.Name) }
            };

            return await UploadAndDeserializeAsync(content, cancellationToken).CAF();
        }

        protected override async Task<IEnumerable<IResult>> DeserializeResponseAsync(
            Stream responseStream, CancellationToken cancellationToken)
        {
            DanbooruResult[]? results = await JsonSerializer.DeserializeAsync<DanbooruResult[]>(
                responseStream,
                cancellationToken: cancellationToken).CAF();

            return results!;
        }
    }
}
