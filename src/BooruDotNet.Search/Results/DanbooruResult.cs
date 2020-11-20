using System.Text.Json.Serialization;
using BooruDotNet.Json;
using BooruDotNet.Posts;

namespace BooruDotNet.Search.Results
{
    public class DanbooruResult : IResult
    {
        public DanbooruResult()
        {
            Post = null!;
        }

        [JsonPropertyName("score")]
        [JsonConverter(typeof(NormalizedPercentageConverter))]
        public double Similarity { get; set; }

        [JsonPropertyName("post")]
        [JsonConverter(typeof(PostConverter<DanbooruPost>))]
        public IPost Post { get; set; }
    }
}
