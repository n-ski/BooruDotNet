using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Json
{
    internal sealed class UnixTimeSecondsToDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long timestamp = reader.GetInt64();

            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(value);
            long timestamp = dateTimeOffset.ToUnixTimeSeconds();

            writer.WriteNumberValue(timestamp);
        }
    }
}
