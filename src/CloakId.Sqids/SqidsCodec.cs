using CloakId.Abstractions;
using Sqids;

namespace CloakId.Sqids;

public class SqidsCodec(
    SqidsEncoder<int> intEncoder,
    SqidsEncoder<uint> uintEncoder,
    SqidsEncoder<long> longEncoder,
    SqidsEncoder<ulong> ulongEncoder,
    SqidsEncoder<short> shortEncoder,
    SqidsEncoder<ushort> ushortEncoder) : ICloakIdCodec
{

    public string Encode(object value, Type valueType)
    {
        var actualType = Nullable.GetUnderlyingType(valueType) ?? valueType;

        return actualType switch
        {
            Type t when t == typeof(int) => intEncoder.Encode((int)value),
            Type t when t == typeof(uint) => uintEncoder.Encode((uint)value),
            Type t when t == typeof(long) => longEncoder.Encode((long)value),
            Type t when t == typeof(ulong) => ulongEncoder.Encode((ulong)value),
            Type t when t == typeof(short) => shortEncoder.Encode((short)value),
            Type t when t == typeof(ushort) => ushortEncoder.Encode((ushort)value),
            _ => throw new NotSupportedException($"Type '{actualType}' is not supported for encoding.")
        };
    }

    public object Decode(string encodedValue, Type targetType)
    {
        var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            return actualType switch
            {
                Type t when t == typeof(int) => intEncoder.Decode(encodedValue)[0],
                Type t when t == typeof(uint) => uintEncoder.Decode(encodedValue)[0],
                Type t when t == typeof(long) => longEncoder.Decode(encodedValue)[0],
                Type t when t == typeof(ulong) => ulongEncoder.Decode(encodedValue)[0],
                Type t when t == typeof(short) => shortEncoder.Decode(encodedValue)[0],
                Type t when t == typeof(ushort) => ushortEncoder.Decode(encodedValue)[0],
                _ => throw new NotSupportedException($"Type '{actualType}' is not supported for decoding.")
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' to type {actualType.Name}.", nameof(encodedValue), ex);
        }
    }
}
