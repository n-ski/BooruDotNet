using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Helpers;
using BooruDotNet.Json;
using BooruDotNet.Resources;

namespace BooruDotNet.Posts
{
    [DebuggerDisplay(IPost.DebuggerDisplayString)]
    public class DanbooruPost : IPost, IExtendedPostTags
    {
        private readonly Lazy<Uri> _postUriLazy;

        public DanbooruPost()
        {
            _postUriLazy = new Lazy<Uri>(() => UriHelpers.CreateFormat(PostUris.Danbooru_Format, ID));
            FileUri = null!;
        }

        #region IPost implementation

        [JsonPropertyName("id")]
        public int? ID { get; set; }

        public Uri Uri => _postUriLazy.Value;

        [JsonPropertyName("created_at")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("image_width")]
        public int Width { get; set; }

        [JsonPropertyName("image_height")]
        public int Height { get; set; }

        [JsonPropertyName("file_size")]
        public long? FileSize { get; set; }

        [JsonPropertyName("md5")]
        public string Hash { get; set; } = "";

        [JsonPropertyName("file_url")]
        public Uri? FileUri { get; set; }

        [JsonPropertyName("large_file_url")]
        public Uri? SampleImageUri { get; set; }

        [JsonPropertyName("preview_file_url")]
        public Uri? PreviewImageUri { get; set; }

        [JsonPropertyName("tag_string")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> Tags { get; set; }

        [JsonPropertyName("rating")]
        [JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        #endregion

        #region IExtendedPostTags implementation

        [JsonPropertyName("tag_string_artist")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> ArtistTags { get; set; }

        [JsonPropertyName("tag_string_character")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> CharacterTags { get; set; }

        [JsonPropertyName("tag_string_copyright")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> CopyrightTags { get; set; }

        [JsonPropertyName("tag_string_general")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> GeneralTags { get; set; }

        [JsonPropertyName("tag_string_meta")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> MetaTags { get; set; }

        #endregion
    }
}
