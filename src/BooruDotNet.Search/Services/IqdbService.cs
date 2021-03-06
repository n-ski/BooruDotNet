using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Extensions;
using BooruDotNet.Search.Extensions;
using BooruDotNet.Search.Resources;
using BooruDotNet.Search.Results;
using HtmlAgilityPack;
using Validation;

namespace BooruDotNet.Search.Services
{
    public class IqdbService : ServiceBase, IFileAndUriSearchService
    {
        private static readonly Lazy<Regex> _widthHeightRegexLazy = new Lazy<Regex>(() =>
            new Regex(@"^(\d+).(\d+) \[\w+\]$", RegexOptions.Compiled));
        private static readonly Lazy<Regex> _similarityRegexLazy = new Lazy<Regex>(() =>
            new Regex(@"^(\d+)% similarity$", RegexOptions.Compiled));
        private readonly bool _enableServiceFlags;

        public IqdbService(HttpClient httpClient)
            : base(httpClient, HttpMethod.Post, new Uri(UploadUris.Iqdb))
        {
            _enableServiceFlags = true;
        }

        /// <remarks>
        /// If this constructor is used, <see cref="SearchAsync(FileStream, IqdbSearchOptions, CancellationToken)"/> and
        /// <see cref="SearchAsync(Uri, IqdbSearchOptions, CancellationToken)"/> will ignore flags that specify
        /// which results will be included in the search.
        /// </remarks>
        public IqdbService(HttpClient httpClient, string prefix)
            : base(httpClient, HttpMethod.Post, CreateUri(prefix))
        {
            _enableServiceFlags = false;
        }

        public long FileSizeLimit => 8 << 20; // 8 MiB.

        public Task<IEnumerable<IResult>> SearchAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return SearchAsync(uri, IqdbSearchOptions.Default, cancellationToken);
        }

        public async Task<IEnumerable<IResult>> SearchAsync(Uri uri, IqdbSearchOptions options, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            using MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new StringContent(uri.AbsoluteUri), "url" }
            };

            if (_enableServiceFlags)
            {
                AppendServiceFlags(content, options);
            }

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<IResult>> SearchAsync(FileStream fileStream, CancellationToken cancellationToken = default)
        {
            return SearchAsync(fileStream, IqdbSearchOptions.Default, cancellationToken);
        }

        public async Task<IEnumerable<IResult>> SearchAsync(FileStream fileStream, IqdbSearchOptions options, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(fileStream, nameof(fileStream));

            Error.If<FileTooLargeException>(
                fileStream.Length > FileSizeLimit,
                fileStream.Name, fileStream.Length, FileSizeLimit);

            using MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "file", "file" }
            };

            if (_enableServiceFlags)
            {
                AppendServiceFlags(content, options);
            }

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<IResult>> DeserializeResponseAsync(
            Stream responseStream, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var doc = new HtmlDocument();
            doc.Load(responseStream);

            // Throw exception if IQDB responded with error.
            if (doc.DocumentNode.SelectSingleNode(@"//div[@class=""err""]") is HtmlNode errorNode)
            {
                throw new HttpRequestException(errorNode.InnerText);
            }

            var divNodes = doc.DocumentNode.SelectNodes(@"//div[@class=""pages""]/div")!;
            var resultsList = new List<IResult>();

            // At index 0 we have uploaded image info, so start from 1 instead.
            for (int i = 1; i < divNodes.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                HtmlNode node = divNodes[i];

                // Skip "No relevant matches" result.
                if (node.Attributes["class"]?.Value is "nomatch")
                {
                    continue;
                }

                HtmlNode tableNode = node.FirstChild;
                HtmlNode? sourceLinkNode = tableNode.SelectSingleNode(@"tr/td[@class=""image""]/a");

                // Skip if there's no link.
                if (sourceLinkNode is null)
                {
                    continue;
                }

                // <a> node - source URI.
                Uri sourceUri = GetUriFromAttribute(sourceLinkNode, "href");

                // <img> node - preview image URI.
                Uri imageUri = GetUriFromAttribute(sourceLinkNode.FirstChild, "src");

                var textNodes = tableNode.SelectNodes(@"tr/td/text()");
                Debug.Assert(textNodes.Count >= 2);

                Match match;

                // <td> node - file width and height.
                match = _widthHeightRegexLazy.Value.Match(textNodes[^2].InnerText);
                int? width = match.Success ? int.Parse(match.Groups[1].Value) : (int?)null;
                int? height = match.Success ? int.Parse(match.Groups[2].Value) : (int?)null;

                // <td> node - similarity.
                match = _similarityRegexLazy.Value.Match(textNodes[^1].InnerText);
                double similarity = double.Parse(match.Groups[1].Value) / 100;

                resultsList.Add(new IqdbResult(
                    sourceUri, imageUri, width, height, similarity));
            }

            return resultsList;
        }

        private Uri GetUriFromAttribute(HtmlNode node, string attribute)
        {
            string? attributeValue = node.Attributes[attribute].Value;

            Debug.Assert(attributeValue is object);

            if (attributeValue.StartsWith("//"))
            {
                attributeValue = $"{UploadUri.Scheme}:{attributeValue}";
            }
            else if (attributeValue.StartsWith("/"))
            {
                attributeValue = $"{UploadUri.Scheme}://{UploadUri.Host}{attributeValue}";
            }

            return new Uri(attributeValue);
        }

        private static Uri CreateUri(string prefix)
        {
            Requires.NotNullOrWhiteSpace(prefix, nameof(prefix));

            string formatted = string.Format(UploadUris.IqdbFormat, prefix);
            return new Uri(formatted);
        }

        private void AppendServiceFlags(MultipartFormDataContent formContent, IqdbSearchOptions flags)
        {
            Debug.Assert(_enableServiceFlags is true);
            Debug.Assert(formContent is object);

            Type flagsType = typeof(IqdbSearchOptions);

            foreach (IqdbSearchOptions flag in Enum.GetValues(flagsType))
            {
                if (flags.HasFlag(flag) && flag.TryGetServiceId(out string? serviceId))
                {
                    formContent.Add(new StringContent(serviceId), "service[]");
                }
            }
        }
    }
}
