using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus
{
    internal sealed class ApiResponseAttributes
    {
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }
    }
}
