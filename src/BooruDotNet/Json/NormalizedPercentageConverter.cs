using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooruDotNet.Json
{
    // Normalizes input number value. 100 will be 1, 50 will be 0.5, etc.
    internal sealed class NormalizedPercentageConverter : JsonConverter<double>
    {
        private const int _target = 100;

        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double value = reader.GetDouble();
            return value / _target;
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            double unnormalized = value * _target;
            writer.WriteNumberValue(unnormalized);
        }
    }
}
