using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LMeter.Converters;

public class Vector4JsonConverter : JsonConverter<Vector4>
{
    public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new Vector4();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            var propertyName = reader.GetString();
            reader.Read();
            // Purposefully chose to have the GetSingle() calls inside of the cases for maintainability
            switch (propertyName)
            {
                case nameof(result.X):
                    var x = reader.GetSingle();
                    result.X = x;
                    break;
                case nameof(result.Y):
                    var y = reader.GetSingle();
                    result.Y = y;
                    break;
                case nameof(result.Z):
                    var z = reader.GetSingle();
                    result.Z = z;
                    break;
                case nameof(result.W):
                    var w = reader.GetSingle();
                    result.W = w;
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber(nameof(value.X), value.X);
        writer.WriteNumber(nameof(value.Y), value.Y);
        writer.WriteNumber(nameof(value.Z), value.Z);
        writer.WriteNumber(nameof(value.W), value.W);

        writer.WriteEndObject();
    }
}