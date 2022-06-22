using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json;

internal sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private const string DateTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string dateString = reader.GetString()!;

        return DateTimeOffset.ParseExact(dateString, DateTimeFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        string dateString = value.ToString(DateTimeFormat);

        writer.WriteStringValue(dateString);
    }
}
