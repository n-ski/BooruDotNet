using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Json;

namespace BooruDotNet.Tags
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    public class GelbooruTag : ITag
    {
        [JsonPropertyName("tag")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        [JsonConverter(typeof(TagKindTextConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        [JsonConverter(typeof(NumberConverter))]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        [JsonConverter(typeof(NumberConverter))]
        public int Count { get; set; }
    }
}
