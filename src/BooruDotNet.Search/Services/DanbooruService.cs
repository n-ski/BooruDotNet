﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Resources;
using BooruDotNet.Search.Resources;
using BooruDotNet.Search.Results;
using Validation;

namespace BooruDotNet.Search.Services
{
    public class DanbooruService : ServiceBase, ISearchByUri, ISearchByFile
    {
        public DanbooruService(HttpClient httpClient)
            : base(httpClient, HttpMethod.Get, new Uri(UploadUris.Danbooru))
        {
        }

        public async Task<IEnumerable<IResult>> SearchByAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            Requires.Argument(uri.IsAbsoluteUri, nameof(uri), ErrorMessages.UriIsNotAbsolute);

            using HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["url"] = uri.AbsoluteUri,
            });

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IResult>> SearchByAsync(
            FileStream fileStream, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(fileStream, nameof(fileStream));

            using HttpContent content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "search[file]", Path.GetFileName(fileStream.Name) }
            };

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<IResult>> DeserializeResponseAsync(
            Stream responseStream, CancellationToken cancellationToken)
        {
            DanbooruResult[]? results = await JsonSerializer.DeserializeAsync<DanbooruResult[]>(
                responseStream,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return results!;
        }
    }
}
