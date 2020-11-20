﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Resources;
using Easy.Common;

namespace BooruDotNet.Search.Services
{
    public abstract class ServiceBase : BooruBase
    {
        protected ServiceBase(HttpMethod uploadMethod, Uri uploadUri)
        {
            UploadMethod = Ensure.NotNull(uploadMethod, nameof(uploadMethod));

            Ensure.NotNull(uploadUri, nameof(uploadUri));
            Ensure.That(uploadUri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);
            UploadUri = uploadUri;
        }

        protected HttpMethod UploadMethod { get; }

        protected Uri UploadUri { get; }

        protected virtual async Task<Stream> UploadContent(HttpContent content, CancellationToken cancellationToken)
        {
            using HttpRequestMessage message = new HttpRequestMessage(UploadMethod, UploadUri)
            {
                Content = content,
            };

            HttpResponseMessage response = await GetResponseAsync(message, cancellationToken);

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
