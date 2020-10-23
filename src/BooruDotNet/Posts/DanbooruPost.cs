﻿using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using BooruDotNet.Json;
using BooruDotNet.Resources;

namespace BooruDotNet.Posts
{
    public class DanbooruPost : IPost
    {
        private readonly Lazy<Uri> _postUriLazy;

        public DanbooruPost()
        {
            _postUriLazy = new Lazy<Uri>(() => new Uri(string.Format(PostUris.Danbooru_Format, ID)));
            FileUri = null!;
        }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        public Uri Uri => _postUriLazy.Value;

        [JsonPropertyName("md5")]
        public string Hash { get; set; } = "";

        [JsonPropertyName("file_url")]
        public Uri FileUri { get; set; }

        [JsonPropertyName("large_file_url")]
        public Uri? SampleImageUri { get; set; }

        [JsonPropertyName("preview_file_url")]
        public Uri? PreviewImageUri { get; set; }

        [JsonPropertyName("tag_string")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> Tags { get; set; }

        [JsonPropertyName("tag_string_artist")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> ArtistTags { get; set; }

        [JsonPropertyName("tag_string_character")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> CharacterTags { get; set; }

        [JsonPropertyName("tag_string_copyright")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> CopyrightTags { get; set; }

        //[JsonPropertyName("tag_string_general")]
        //[JsonConverter(typeof(TagStringConverter))]
        //public ImmutableArray<string> GeneralTags { get; set; }

        [JsonPropertyName("tag_string_meta")]
        [JsonConverter(typeof(TagStringConverter))]
        public ImmutableArray<string> MetaTags { get; set; }
    }
}
