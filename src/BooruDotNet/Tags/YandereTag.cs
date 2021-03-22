using System.Text.Json.Serialization;
using BooruDotNet.Json;

namespace BooruDotNet.Tags
{
    internal sealed class YandereTag : ITag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type"), JsonConverter(typeof(YandereTagKindConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
