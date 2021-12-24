using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Extensions;
using BooruDotNet.Search.Results;
using Validation;

namespace BooruDotNet.Search.Services
{
    public abstract class ServiceBase : BooruBase
    {
        protected ServiceBase(HttpClient httpClient, HttpMethod uploadMethod, Uri uploadUri)
            : base(httpClient)
        {
            UploadMethod = Requires.NotNull(uploadMethod, nameof(uploadMethod));

            Requires.NotNull(uploadUri, nameof(uploadUri));
            uploadUri.RequireAbsolute(nameof(uploadUri));

            UploadUri = uploadUri;
        }

        protected HttpMethod UploadMethod { get; }

        protected Uri UploadUri { get; }

        protected async Task<IEnumerable<IResult>> UploadAndDeserializeAsync(
            HttpContent content, CancellationToken cancellationToken)
        {
            using Stream responseStream = await UploadContentAsync(content, cancellationToken).CAF();

            return await DeserializeResponseAsync(responseStream, cancellationToken).CAF();
        }

        protected virtual async Task<Stream> UploadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            using HttpRequestMessage message = new HttpRequestMessage(UploadMethod, UploadUri)
            {
                Content = content,
            };

            HttpResponseMessage response = await HttpClient.SendAsync(message, cancellationToken).CAF();

            return await response.Content.ReadAsStreamAsync().CAF();
        }

        protected abstract Task<IEnumerable<IResult>> DeserializeResponseAsync(
            Stream responseStream, CancellationToken cancellationToken);
    }
}
