using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Boorus.Json;
using BooruDotNet.Boorus.Resources;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;

namespace BooruDotNet.Boorus;

[DebuggerDisplay(IPost.DebuggerDisplayString)]
internal sealed class DanbooruPost : IPost, IPostExtraTags
{
    private readonly Lazy<Uri?> _postUriLazy;

    public DanbooruPost()
    {
        _postUriLazy = new Lazy<Uri?>(() => ID.HasValue ? UriHelper.CreateFormat(Uris.Danbooru_PostUri_Format, ID) : null);

        string[] emptyTags = Array.Empty<string>();
        Tags = emptyTags;
        ArtistTags = emptyTags;
        CharacterTags = emptyTags;
        CopyrightTags = emptyTags;
        GeneralTags = emptyTags;
        MetaTags = emptyTags;
    }

    #region IPost implementation

    [JsonPropertyName("id")]
    public int? ID { get; set; }

    public Uri? Uri => _postUriLazy.Value;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreationDate { get; set; }

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
    public IReadOnlyList<string> Tags { get; set; }

    [JsonPropertyName("rating")]
    [JsonConverter(typeof(DanbooruRatingConverter))]
    public Rating Rating { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";

    [JsonPropertyName("score")]
    public int? Score { get; set; }

    #endregion

    #region IPostExtraTags implementation

    [JsonPropertyName("tag_string_artist")]
    [JsonConverter(typeof(TagStringConverter))]
    public IReadOnlyList<string> ArtistTags { get; set; }

    [JsonPropertyName("tag_string_character")]
    [JsonConverter(typeof(TagStringConverter))]
    public IReadOnlyList<string> CharacterTags { get; set; }

    [JsonPropertyName("tag_string_copyright")]
    [JsonConverter(typeof(TagStringConverter))]
    public IReadOnlyList<string> CopyrightTags { get; set; }

    [JsonPropertyName("tag_string_general")]
    [JsonConverter(typeof(TagStringConverter))]
    public IReadOnlyList<string> GeneralTags { get; set; }

    [JsonPropertyName("tag_string_meta")]
    [JsonConverter(typeof(TagStringConverter))]
    public IReadOnlyList<string> MetaTags { get; set; }

    #endregion
}
