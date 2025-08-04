using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using CloakId.Abstractions;

namespace CloakId;

/// <summary>
/// JSON converter modifier that handles properties marked with [Cloak] attribute.
/// </summary>
public class CloakIdTypeInfoResolver(ICloakIdCodec codec) : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object)
        {
            foreach (var propertyInfo in jsonTypeInfo.Properties)
            {
                var propertyType = propertyInfo.PropertyType;
                var property = type.GetProperty(propertyInfo.Name);

                if (property?.GetCustomAttribute<CloakAttribute>() != null &&
                    IsNumericType(propertyType))
                {
                    propertyInfo.CustomConverter = new CloakIdPropertyConverter(propertyType, codec);
                }
            }
        }

        return jsonTypeInfo;
    }

    private static bool IsNumericType(Type type)
    {
        var actualType = Nullable.GetUnderlyingType(type) ?? type;
        return actualType == typeof(int) ||
               actualType == typeof(uint) ||
               actualType == typeof(long) ||
               actualType == typeof(ulong) ||
               actualType == typeof(short) ||
               actualType == typeof(ushort);
    }
}
