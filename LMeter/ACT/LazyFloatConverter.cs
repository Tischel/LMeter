
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LMeter.ACT
{
    public class LazyFloatConverter : JsonConverter<LazyFloat>
    {
        public override void Write(Utf8JsonWriter writer, LazyFloat value, JsonSerializerOptions serializer)
        {
            throw new NotImplementedException("Write not supported.");
        }

        public override LazyFloat Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializerOptions)
        {
            if (objectType != typeof(LazyFloat))
            {
                throw new NotImplementedException("Unknown Type found");
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                return new LazyFloat(0f);
            }

            return new LazyFloat(reader.GetString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LazyFloat);
        }
    }
}