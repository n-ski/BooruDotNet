using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Validation;

namespace BooruDotNet.Boorus.Json
{
    internal sealed class TagKindConverter : JsonConverter<TagKind>
    {
        public override TagKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int numericValue = reader.GetInt32();

            return numericValue switch
            {
                0 => TagKind.General,
                1 => TagKind.Artist,
                2 => TagKind.General, // Studio
                3 => TagKind.Copyright,
                4 => TagKind.Character,
                5 => TagKind.General, // Genre
                8 => TagKind.General, // Medium
                9 => TagKind.Metadata,
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, TagKind value, JsonSerializerOptions options)
        {
            int numericValue = value switch
            {
                TagKind.General => 0,
                TagKind.Artist => 1,
                TagKind.Copyright => 3,
                TagKind.Character => 4,
                TagKind.Metadata => 8,
                _ => throw Assumes.NotReachable(),
            };

            writer.WriteNumberValue(numericValue);
        }
    }
}
