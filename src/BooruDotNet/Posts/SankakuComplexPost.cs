using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Helpers;
using BooruDotNet.Json;
using BooruDotNet.Resources;
using BooruDotNet.Tags;

namespace BooruDotNet.Posts
{
    [DebuggerDisplay(IPost.DebuggerDisplayString)]
    internal sealed class SankakuComplexPost : IPost, IPostExtendedTags
    {
        private readonly Lazy<Uri?> _postUriLazy;
        private readonly Lazy<IReadOnlyList<string>> _tagsLazy;

        public SankakuComplexPost()
        {
            _postUriLazy = new Lazy<Uri?>(() => ID.HasValue ? UriHelper.CreateFormat(PostUris.SankakuComplex_Format, ID) : null);

            Hash = string.Empty;
            Source = string.Empty;
            ExtendedTags = Array.Empty<ITag>();

            _tagsLazy = new Lazy<IReadOnlyList<string>>(() =>
            {
                string[] tagNames = new string[ExtendedTags.Count];
                for (int i = 0; i < tagNames.Length; i++)
                {
                    tagNames[i] = ExtendedTags[i].Name;
                }

                return tagNames;
            });
        }

        #region Helper classes

        internal class CreationDateInfo
        {
            [JsonPropertyName("s"), JsonConverter(typeof(UnixTimeSecondsToDateConverter))]
            public DateTime CreationDate { get; set; }
        }

        #endregion

        #region IPost implementation

        [JsonPropertyName("id")]
        public int? ID { get; set; }

        public Uri? Uri => _postUriLazy.Value;

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

        public IReadOnlyList<string> Tags => _tagsLazy.Value;

        [JsonPropertyName("rating"), JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("total_score")]
        public int? Score { get; set; }

        #endregion

        #region IPostExtendedTags implementation

        [JsonPropertyName("tags"), JsonConverter(typeof(TagCollectionConverter<SankakuComplexTag>))]
        public IReadOnlyList<ITag> ExtendedTags { get; set; }

        #endregion

        [JsonPropertyName("created_at")]
        public CreationDateInfo SankakuCreationDate
        {
            set => CreationDate = value.CreationDate;
        }
    }
}
