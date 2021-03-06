using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Gelbooru.Json
{
    internal sealed class GelbooruTagKindConverter : JsonConverter<TagKind>
    {
        public override TagKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string tagKindString = reader.GetString()!;

            if (Enum.TryParse(tagKindString, true, out TagKind tagKind))
            {
                return tagKind;
            }
            else if (tagKindString.Equals("tag", StringComparison.OrdinalIgnoreCase))
            {
                return TagKind.General;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, TagKind value, JsonSerializerOptions options)
        {
            string tagKindString = value == TagKind.General ? "tag" : value.ToString().ToLowerInvariant();

            writer.WriteStringValue(tagKindString);
        }
    }
}
