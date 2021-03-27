using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Gelbooru.Json
{
    internal sealed class GelbooruDateTimeConverter : JsonConverter<DateTime>
    {
        private const string _dateTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;

            return DateTime.ParseExact(dateString, _dateTimeFormat, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            string dateString = value.ToString(_dateTimeFormat);

            writer.WriteStringValue(dateString);
        }
    }
}
