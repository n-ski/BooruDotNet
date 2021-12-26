using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Boorus.Json;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;

namespace BooruDotNet.Boorus
{
    [DebuggerDisplay(IPost.DebuggerDisplayString)]
    internal sealed class GelbooruPost : IPost
    {
        private readonly Lazy<Uri?> _postUriLazy;
        private readonly Lazy<Uri?> _sampleImageUriLazy;
        private readonly Lazy<Uri> _previewImageUriLazy;

        public GelbooruPost()
        {
            _postUriLazy = new Lazy<Uri?>(() => ID.HasValue ? UriHelper.CreateFormat(Uris.Gelbooru_PostUri_Format, ID) : null);
            _sampleImageUriLazy = new Lazy<Uri?>(() =>
            {
                if (Sample == 1)
                {
                    return new Uri($"https://img3.gelbooru.com//samples/{Directory}/sample_{Hash}.jpg");
                }
                else
                {
                    return FileUri;
                }
            });
            _previewImageUriLazy = new Lazy<Uri>(() => new Uri($"https://img1.gelbooru.com/thumbnails/{Directory}/thumbnail_{Hash}.jpg"));

            Tags = Array.Empty<string>();
        }

        [JsonPropertyName("id")]
        public int? ID { get; set; }

        public Uri? Uri => _postUriLazy.Value;

        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(GelbooruDateTimeOffsetConverter))]
        public DateTimeOffset CreationDate { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        public long? FileSize => null;

        [JsonPropertyName("hash")]
        public string Hash { get; set; } = "";

        [JsonPropertyName("file_url")]
        public Uri? FileUri { get; set; }

        public Uri? SampleImageUri => _sampleImageUriLazy.Value;

        public Uri? PreviewImageUri => _previewImageUriLazy.Value;

        [JsonPropertyName("tags")]
        [JsonConverter(typeof(TagStringConverter))]
        public IReadOnlyList<string> Tags { get; set; }

        [JsonPropertyName("rating")]
        [JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("directory")]
        public string Directory { get; set; } = "";

        [JsonPropertyName("sample")]
        public int Sample { get; set; }
    }
}
