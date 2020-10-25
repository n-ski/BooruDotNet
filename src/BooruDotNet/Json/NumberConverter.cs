using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Json
{
    internal sealed class NumberConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            return int.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            string stringified = value.ToString();

            writer.WriteStringValue(stringified);
        }
    }
}
