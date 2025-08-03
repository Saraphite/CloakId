using System.Text.Json;
using System.Text.Json.Serialization;
using CloakId.Abstractions;

namespace CloakId;

/// <summary>
/// JSON converter for properties marked with [CloakId].
/// </summary>
public class CloakIdPropertyConverter(Type propertyType, ICloakIdCodec codec) : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            if (Nullable.GetUnderlyingType(propertyType) != null) return null;
            throw new JsonException($"Cannot convert null to non-nullable type {propertyType}");
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var encodedValue = reader.GetString()!;
            return codec.Decode(encodedValue, propertyType);
        }

        throw new JsonException($"Expected string token for CloakId property of type {propertyType}, but got {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var encoded = codec.Encode(value, propertyType);
        writer.WriteStringValue(encoded);
    }
}
