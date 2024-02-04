using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LMeter.Helpers;

namespace LMeter.Converters;

public class FontDataJsonConverter : JsonConverter<FontData>
{
    public override FontData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new FontData();
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
                case nameof(result.Name):
                    var name = reader.GetString() ?? "ERROR";
                    result.Name = name;
                    break;
                case nameof(result.Size):
                    var size = reader.GetInt32();
                    result.Size = size;
                    break;
                case nameof(result.Chinese):
                    var isChinese = reader.GetBoolean();
                    result.Chinese = isChinese;
                    break;
                case nameof(result.Korean):
                    var isKorean = reader.GetBoolean();
                    result.Korean = isKorean;
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, FontData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString(nameof(value.Name), value.Name);
        writer.WriteNumber(nameof(value.Size), value.Size);
        writer.WriteBoolean(nameof(value.Chinese), value.Chinese);
        writer.WriteBoolean(nameof(value.Korean), value.Korean);

        writer.WriteEndObject();
    }
}