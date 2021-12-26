using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json
{
    internal sealed class RatingConverter : JsonConverter<Rating>
    {
        public override Rating Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string ratingString = reader.GetString()!;

            if (ratingString.Length > 1)
            {
                return (Rating)Enum.Parse(typeof(Rating), ratingString, true);
            }
            else if (ratingString.Length == 1)
            {
                switch (ratingString[0])
                {
                    case 's':
                    case 'S':
                        return Rating.Safe;

                    case 'q':
                    case 'Q':
                        return Rating.Questionable;

                    case 'e':
                    case 'E':
                        return Rating.Explicit;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Rating value, JsonSerializerOptions options)
        {
            string ratingString = value.ToString().ToLowerInvariant();

            writer.WriteStringValue(ratingString);
        }
    }
}
