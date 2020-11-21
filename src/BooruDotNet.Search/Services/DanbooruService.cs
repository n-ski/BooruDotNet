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
using Easy.Common;

namespace BooruDotNet.Search.Services
{
    public class DanbooruService : ServiceBase, ISearchByUri, ISearchByFile
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

            return await UploadAndDeserialize(content, cancellationToken);
        }

        public async Task<IEnumerable<IResult>> SearchBy(FileStream fileStream, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(fileStream, nameof(fileStream));

            using HttpContent content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "search[file]", Path.GetFileName(fileStream.Name) }
            };

            return await UploadAndDeserialize(content, cancellationToken);
        }

        private async Task<DanbooruResult[]> UploadAndDeserialize(HttpContent content, CancellationToken cancellationToken)
        {
            using Stream responseStream = await UploadContent(content, cancellationToken);

            return await JsonSerializer.DeserializeAsync<DanbooruResult[]>(responseStream, cancellationToken: cancellationToken);
        }
    }
}
