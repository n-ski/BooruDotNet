using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Gelbooru.Json;
using BooruDotNet.Tags;

namespace BooruDotNet.Gelbooru
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    internal sealed class GelbooruTag : ITag
    {
        [JsonPropertyName("tag")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        [JsonConverter(typeof(GelbooruTagKindConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
