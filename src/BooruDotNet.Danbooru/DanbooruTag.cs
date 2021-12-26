using System.Diagnostics;
using System.Text.Json.Serialization;
using BooruDotNet.Boorus.Json;
using BooruDotNet.Tags;

namespace BooruDotNet.Boorus
{
    [DebuggerDisplay(ITag.DebuggerDisplayString)]
    internal sealed class DanbooruTag : ITag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("category")]
        [JsonConverter(typeof(TagKindConverter))]
        public TagKind Kind { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("post_count")]
        public int Count { get; set; }
    }
}
