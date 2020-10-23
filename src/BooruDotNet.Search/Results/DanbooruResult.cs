using System;
using System.Text.Json.Serialization;
using BooruDotNet.Boorus;
using BooruDotNet.Posts;
using BooruDotNet.Search.Json;

namespace BooruDotNet.Search.Results
{
    internal sealed class DanbooruResult : IResult, IResultWithPost
    {
        public DanbooruResult()
        {
            Post = null!;
        }

        #region IResult implementation

        public Uri Source => Post.Uri!;

        public Uri PreviewImageUri => Post.PreviewImageUri!;

        public int? Width => Post.Width;

        public int? Height => Post.Height;

        [JsonPropertyName("score")]
        [JsonConverter(typeof(NormalizedPercentageConverter))]
        public double Similarity { get; set; }

        #endregion

        #region IResultWithPost implementation

        [JsonPropertyName("post")]
        [JsonConverter(typeof(PostConverter<DanbooruPost>))]
        public IPost Post { get; set; }

        #endregion
    }
}
