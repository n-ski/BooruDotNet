using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json
{
    internal sealed class DanbooruTagKindConverter : JsonConverter<TagKind>
    {
        public override TagKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int value = reader.GetInt32();

            if (Enum.IsDefined(typeof(TagKind), value))
            {
                return (TagKind)value;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TagKind value, JsonSerializerOptions options)
        {
            int number = (int)value;

            writer.WriteNumberValue(number);
        }
    }
}
