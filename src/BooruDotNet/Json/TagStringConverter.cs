﻿using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Json
{
    internal sealed class TagStringConverter : JsonConverter<ImmutableArray<string>>
    {
        private static readonly char _separator = ' ';

        public override ImmutableArray<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string tagString = reader.GetString();
            string[] tags = tagString.Split(_separator);

            return ImmutableArray.Create(tags);
        }

        public override void Write(Utf8JsonWriter writer, ImmutableArray<string> value, JsonSerializerOptions options)
        {
            string tagString = string.Join(_separator, value);

            writer.WriteStringValue(tagString);
        }
    }
}