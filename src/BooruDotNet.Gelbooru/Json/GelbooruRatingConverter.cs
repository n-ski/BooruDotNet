using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json;

internal sealed class GelbooruRatingConverter : JsonConverter<Rating>
{
    public override Rating Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? ratingString = reader.GetString();

        return ratingString switch
        {
            "sensitive"    => Rating.Sensitive,
            "questionable" => Rating.Questionable,
            "general"      => Rating.General,
            "explicit"     => Rating.Explicit,
            _              => throw new JsonException($"Couldn't convert rating '{ratingString}' to an enum value."),
        };
    }

    public override void Write(Utf8JsonWriter writer, Rating value, JsonSerializerOptions options)
    {
        string ratingString = value.ToString().ToLowerInvariant();

        writer.WriteStringValue(ratingString);
    }
}
