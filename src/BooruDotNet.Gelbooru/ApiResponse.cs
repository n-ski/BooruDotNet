using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus
{
    internal sealed class ApiResponse
    {
#if DEBUG
        [JsonPropertyName("@attributes")]
        public ApiResponseAttributes? Attributes { get; set; }
#endif

        [JsonPropertyName("post")]
        public GelbooruPost[]? Posts { get; set; }

        [JsonPropertyName("tag")]
        public GelbooruTag[]? Tags { get; set; }
    }
}
