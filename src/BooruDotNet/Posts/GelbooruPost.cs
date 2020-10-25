using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using BooruDotNet.Helpers;
using BooruDotNet.Json;
using BooruDotNet.Resources;

namespace BooruDotNet.Posts
{
    public class GelbooruPost : IPost
    {
        private readonly Lazy<Uri> _postUriLazy;
        private readonly Lazy<Uri> _sampleImageUriLazy;
        private readonly Lazy<Uri> _previewImageUriLazy;

        public GelbooruPost()
        {
            _postUriLazy = new Lazy<Uri>(() => UriHelpers.CreateFormat(PostUris.Gelbooru_Format, ID));
            _sampleImageUriLazy = new Lazy<Uri>(() => new Uri($"https://img2.gelbooru.com/samples/{Directory}/sample_{Hash}.jpg"));
            _previewImageUriLazy = new Lazy<Uri>(() => new Uri($"https://img1.gelbooru.com/thumbnails/{Directory}/thumbnail_{Hash}.jpg"));

            FileUri = null!;
        }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        public Uri Uri => _postUriLazy.Value;

        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(GelbooruDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        public long? FileSize => null;

        [JsonPropertyName("hash")]
        public string Hash { get; set; } = "";

        [JsonPropertyName("file_url")]
        public Uri FileUri { get; set; }

        public Uri? SampleImageUri => _sampleImageUriLazy.Value;

        public Uri? PreviewImageUri => _previewImageUriLazy.Value;

        [JsonPropertyName("tags")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> Tags { get; set; }

        [JsonPropertyName("rating")]
        [JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("directory")]
        public string Directory { get; set; } = "";
    }
}
