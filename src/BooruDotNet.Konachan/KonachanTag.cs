using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Konachan.Json;
using BooruDotNet.Tags;

namespace BooruDotNet.Konachan
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    internal sealed class KonachanTag : ITag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type"), JsonConverter(typeof(KonachanTagKindConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
