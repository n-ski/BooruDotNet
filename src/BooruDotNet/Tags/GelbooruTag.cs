using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Json;

namespace BooruDotNet.Tags
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    internal sealed class GelbooruTag : ITag
    {
        [JsonPropertyName("tag")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        [JsonConverter(typeof(TagKindTextConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
