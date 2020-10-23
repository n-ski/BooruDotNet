using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Boorus.Json;
using BooruDotNet.Tags;

namespace BooruDotNet.Boorus
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
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
