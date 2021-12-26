using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using BooruDotNet.Tags;

namespace BooruDotNet.Boorus.Json
{
    internal sealed class TagCollectionConverter<T> : JsonConverter<IReadOnlyList<ITag>> where T : ITag
    {
        public override IReadOnlyList<ITag> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<T> tags = (List<T>)JsonSerializer.Deserialize(ref reader, typeof(List<T>), options)!;

            tags.Sort((firstTag, secondTag) => firstTag.Name.CompareTo(secondTag.Name));

            return (IReadOnlyList<ITag>)tags;
        }

        public override void Write(Utf8JsonWriter writer, IReadOnlyList<ITag> value, JsonSerializerOptions options)
        {
            if (value.GetType().IsAssignableFrom(typeof(IReadOnlyList<T>)))
            {
                JsonSerializer.Serialize(writer, value, typeof(IReadOnlyList<T>), options);
            }

            throw new JsonException();
        }
    }
}
