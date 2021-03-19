using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Json;

namespace BooruDotNet.Tags
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    internal sealed class SankakuComplexTag : ITag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type"), JsonConverter(typeof(SankakuComplexTagKindConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
