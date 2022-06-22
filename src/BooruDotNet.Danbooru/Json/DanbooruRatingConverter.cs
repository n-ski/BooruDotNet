using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json;

internal sealed class DanbooruRatingConverter : JsonConverter<Rating>
{
    public override Rating Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? ratingString = reader.GetString();
        char? ratingChar = ratingString?.Length > 0 ? ratingString[0] : (char?)null;

        return ratingChar switch
        {
            'e' => Rating.Explicit,
            'g' => Rating.General,
            'q' => Rating.Questionable,
            's' => Rating.Sensitive,
            _ => throw new JsonException($"Couldn't convert rating '{ratingChar}' to an enum value."),
        };
    }

    public override void Write(Utf8JsonWriter writer, Rating value, JsonSerializerOptions options)
    {
        string? rating = value switch
        {
            Rating.Explicit     => "e",
            Rating.General      => "g",
            Rating.Questionable => "q",
            Rating.Sensitive    => "s",
            _                   => null,
        };

        writer.WriteStringValue(rating);
    }
}
