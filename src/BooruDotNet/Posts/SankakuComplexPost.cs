using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Helpers;
using BooruDotNet.Json;
using BooruDotNet.Resources;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Posts
{
    [DebuggerDisplay(IPost.DebuggerDisplayString)]
    internal sealed class SankakuComplexPost : IPost, IPostExtendedTags
    {
        private readonly Lazy<Uri?> _postUriLazy;

        public SankakuComplexPost()
        {
            _postUriLazy = new Lazy<Uri?>(() => ID.HasValue ? UriHelper.CreateFormat(PostUris.SankakuComplex_Format, ID) : null);

            Hash = string.Empty;
            Source = string.Empty;

            string[] emptyTags = Array.Empty<string>();
            Tags = emptyTags;
            ArtistTags = emptyTags;
            CharacterTags = emptyTags;
            CopyrightTags = emptyTags;
            GeneralTags = emptyTags;
            MetaTags = emptyTags;
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

        public IReadOnlyList<string> Tags { get; set; }

        [JsonPropertyName("rating"), JsonConverter(typeof(RatingConverter))]
        public Rating Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("total_score")]
        public int? Score { get; set; }

        #endregion

        #region IPostExtendedTags implementation

        public IReadOnlyList<string> ArtistTags { get; set; }

        public IReadOnlyList<string> CharacterTags { get; set; }

        public IReadOnlyList<string> CopyrightTags { get; set; }

        public IReadOnlyList<string> GeneralTags { get; set; }

        public IReadOnlyList<string> MetaTags { get; set; }

        #endregion

        [JsonPropertyName("created_at")]
        public CreationDateInfo SankakuCreationDate
        {
            set => CreationDate = value.CreationDate;
        }

        [JsonPropertyName("tags")]
        public IReadOnlyList<SankakuComplexTag> SankakuTags
        {
            // For some stupid reason this needs to have "get" accessor,
            // otherwise "set" won't be called on deserialization.
            get => throw Assumes.NotReachable();
            set
            {
                var tags = new List<string>(value.Count);
                var artists = new List<string>();
                var characters = new List<string>();
                var copyrights = new List<string>();
                var general = new List<string>();
                var meta = new List<string>();

                foreach (ITag tag in value)
                {
                    tags.Add(tag.Name);

                    switch (tag.Kind)
                    {
                        case TagKind.General:
                            general.Add(tag.Name);
                            break;
                        case TagKind.Artist:
                            artists.Add(tag.Name);
                            break;
                        case TagKind.Copyright:
                            copyrights.Add(tag.Name);
                            break;
                        case TagKind.Character:
                            characters.Add(tag.Name);
                            break;
                        case TagKind.Metadata:
                            meta.Add(tag.Name);
                            break;
                    }
                }

                tags.Sort();

                Tags = tags;
                ArtistTags = artists;
                CharacterTags = characters;
                CopyrightTags = copyrights;
                GeneralTags = general;
                MetaTags = meta;
            }
        }
    }
}
