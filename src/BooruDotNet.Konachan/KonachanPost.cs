using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Helpers;
using BooruDotNet.Json;
using BooruDotNet.Konachan.Resources;
using BooruDotNet.Posts;

namespace BooruDotNet.Konachan
{
    [DebuggerDisplay(IPost.DebuggerDisplayString)]
    internal sealed class KonachanPost : IPost
    {
        private readonly Lazy<Uri?> _postUriLazy;

        public KonachanPost()
        {
            _postUriLazy = new Lazy<Uri?>(() => ID.HasValue ? UriHelper.CreateFormat(Uris.Konachan_PostUri_Format, ID) : null);

            Hash = string.Empty;
            Source = string.Empty;
            Tags = Array.Empty<string>();
        }

        [JsonPropertyName("id")]
        public int? ID { get; set; }

        public Uri? Uri => _postUriLazy.Value;

        [JsonPropertyName("created_at"), JsonConverter(typeof(UnixTimeSecondsToDateConverter))]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("file_size")]
        public long? FileSize { get; set; }

        [JsonPropertyName("md5")]
        public string Hash { get; set; }

        [JsonPropertyName("file_url")]
        public Uri? FileUri { get; set; }

        [JsonPropertyName("sample_url")]
        public Uri? SampleImageUri { get; set; }

        [JsonPropertyName("preview_url")]
        public Uri? PreviewImageUri { get; set; }

        [JsonPropertyName("tags"), JsonConverter(typeof(TagStringConverter))]
        public IReadOnlyList<string> Tags { get; set; }

        [JsonPropertyName("rating"), JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("score")]
        public int? Score { get; set; }
    }
}
