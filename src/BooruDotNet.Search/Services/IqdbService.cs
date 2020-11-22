using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Extensions;
using BooruDotNet.Search.Resources;
using BooruDotNet.Search.Results;
using HtmlAgilityPack;
using Validation;

namespace BooruDotNet.Search.Services
{
    public class IqdbService : ServiceBase, ISearchByUriAndFile
    {
        private static readonly Lazy<Regex> _widthHeightRegexLazy = new Lazy<Regex>(() =>
            new Regex(@"^(\d+).(\d+) \[\w+\]$", RegexOptions.Compiled));
        private static readonly Lazy<Regex> _similarityRegexLazy = new Lazy<Regex>(() =>
            new Regex(@"^(\d+)% similarity$", RegexOptions.Compiled));

        // TODO: fix deserialization logic and then make public.
        internal IqdbService(HttpClient httpClient)
            : base(httpClient, HttpMethod.Post, new Uri(UploadUris.Iqdb))
        {
        }

        public IqdbService(HttpClient httpClient, string prefix)
            : base(httpClient, HttpMethod.Post, CreateUri(prefix))
        {
        }

        public async Task<IEnumerable<IResult>> SearchByAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            using HttpContent content = new MultipartFormDataContent
            {
                { new StringContent(uri.AbsoluteUri), "url" }
            };

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IResult>> SearchByAsync(FileStream fileStream, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(fileStream, nameof(fileStream));

            using HttpContent content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "file", "file" }
            };

            return await UploadAndDeserializeAsync(content, cancellationToken).ConfigureAwait(false);
        }

        protected override Task<IEnumerable<IResult>> DeserializeResponseAsync(
            Stream responseStream, CancellationToken cancellationToken)
        {
            var doc = new HtmlDocument();
            doc.Load(responseStream);

            // Throw exception if IQDB responded with error.
            if (doc.DocumentNode.SelectSingleNode(@"//div[@class=""err""]") is HtmlNode errorNode)
            {
                throw new HttpRequestException(errorNode.InnerText);
            }

            // Skip uploaded image info.
            var tables = doc.DocumentNode.SelectNodes(@"//*[@class=""pages""]/div/table")?.Skip(1);

            // TODO: cancellation.
            IEnumerable<IResult> results = tables
                .Select(table =>
                {
                    HtmlNode currentNode = table.FirstChild;

                    // Skip one node if header is present.
                    if (currentNode.FirstChild.Name == "th")
                    {
                        currentNode = currentNode.NextSibling;
                    }

                    HtmlNode linkNode = currentNode.FirstChild.FirstChild;

                    // Skip if there's no link.
                    // TODO: for multi-service search, skip additional rows.
                    if (linkNode.Name != "a")
                    {
                        return null!;
                    }

                    // <a> node - source URI.
                    Uri sourceUri = GetUriFromAttribute(linkNode, "href");

                    // <img> node - preview image URI.
                    Uri imageUri = GetUriFromAttribute(linkNode.FirstChild, "src");

                    Match match;

                    // <td> node - file width and height.
                    currentNode = currentNode.NextSibling;
                    match = _widthHeightRegexLazy.Value.Match(currentNode.InnerText);
                    int width = int.Parse(match.Groups[1].Value);
                    int height = int.Parse(match.Groups[2].Value);

                    // <td> node - similarity.
                    currentNode = currentNode.NextSibling;
                    match = _similarityRegexLazy.Value.Match(currentNode.InnerText);
                    double similarity = double.Parse(match.Groups[1].Value) / 100;

                    return new IqdbResult(sourceUri, imageUri, width, height, similarity);
                })
                .Where(result => !(result is null))
                .ToArray();

            return Task.FromResult(results);
        }

        private Uri GetUriFromAttribute(HtmlNode node, string attribute)
        {
            string href = node.Attributes[attribute].Value;

            if (href.StartsWith("//"))
            {
                href = $"{UploadUri.Scheme}:{href}";
            }
            else if (href.StartsWith("/"))
            {
                href = $"{UploadUri.Scheme}://{UploadUri.Host}{href}";
            }

            return new Uri(href);
        }

        private static Uri CreateUri(string prefix)
        {
            Requires.NotNullOrWhiteSpace(prefix, nameof(prefix));

            string formatted = string.Format(UploadUris.IqdbFormat, prefix);
            return new Uri(formatted);
        }
    }
}
