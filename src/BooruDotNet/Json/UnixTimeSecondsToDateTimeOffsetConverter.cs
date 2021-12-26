using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Json
{
    internal sealed class UnixTimeSecondsToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long timestamp = reader.GetInt64();

            return DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            long timestamp = value.ToUnixTimeSeconds();

            writer.WriteNumberValue(timestamp);
        }
    }
}
