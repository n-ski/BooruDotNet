using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Tags;
using BooruDotNet.Yandere.Json;

namespace BooruDotNet.Yandere
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
