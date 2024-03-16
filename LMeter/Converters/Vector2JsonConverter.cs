using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LMeter.Converters;

public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        Vector2 result = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            string? propertyName = reader.GetString();
            reader.Read();
            // Purposefully chose to have the GetSingle() calls inside of the cases for maintainability
            switch (propertyName)
            {
                case nameof(result.X):
                    float x = reader.GetSingle();
                    result.X = x;
                    break;
                case nameof(result.Y):
                    float y = reader.GetSingle();
                    result.Y = y;
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber(nameof(value.X), value.X);
        writer.WriteNumber(nameof(value.Y), value.Y);

        writer.WriteEndObject();
    }
}