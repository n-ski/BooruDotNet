﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Boorus.Json;

internal sealed class TagStringConverter : JsonConverter<IReadOnlyList<string>>
{
    private static readonly char Separator = ' ';

    public override IReadOnlyList<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string tagString = reader.GetString()!;

        if (tagString is null)
        {
            throw new JsonException();
        }

        return tagString.Length > 0
            ? tagString.Split(Separator)
            : Array.Empty<string>();
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<string> value, JsonSerializerOptions options)
    {
        string tagString = string.Join(Separator, value);

        writer.WriteStringValue(tagString);
    }
}
