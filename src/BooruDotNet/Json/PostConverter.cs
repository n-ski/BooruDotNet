using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BooruDotNet.Posts;
using BooruDotNet.Resources;

namespace BooruDotNet.Json
{
    internal sealed class PostConverter<T> : JsonConverter<IPost> where T : IPost
    {
        public override IPost Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (IPost)JsonSerializer.Deserialize(ref reader, typeof(T), options);
        }

        public override void Write(Utf8JsonWriter writer, IPost value, JsonSerializerOptions options)
        {
            if (value is T)
            {
                JsonSerializer.Serialize(writer, value, typeof(T), options);
            }

            throw new JsonException(string.Format(
                ErrorMessages.PostConverterInvalidTypeFormat,
                typeof(T),
                value.GetType()));
        }
    }
}
