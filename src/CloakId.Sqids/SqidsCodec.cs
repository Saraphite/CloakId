using CloakId.Abstractions;
using Sqids;

namespace CloakId.Sqids;

/// <summary>
/// Sqids implementation of ICloakIdCodec for encoding and decoding integer IDs.
/// </summary>
public class SqidsCodec(
    SqidsEncoder<int> intEncoder,
    SqidsEncoder<uint> uintEncoder,
    SqidsEncoder<long> longEncoder,
    SqidsEncoder<ulong> ulongEncoder,
    SqidsEncoder<short> shortEncoder,
    SqidsEncoder<ushort> ushortEncoder) : ICloakIdCodec
{
    /// <summary>
    /// Encodes a numeric value to a string using Sqids.
    /// </summary>
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

    /// <summary>
    /// Decodes a Sqids string back to the original numeric value.
    /// Validates that the input is the canonical encoding to prevent multiple IDs resolving to the same value.
    /// </summary>
    public object Decode(string encodedValue, Type targetType)
    {
        var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            var (decodedValue, canonicalEncoding) = actualType switch
            {
                Type t when t == typeof(int) => DecodeInt(encodedValue),
                Type t when t == typeof(uint) => DecodeUInt(encodedValue),
                Type t when t == typeof(long) => DecodeLong(encodedValue),
                Type t when t == typeof(ulong) => DecodeULong(encodedValue),
                Type t when t == typeof(short) => DecodeShort(encodedValue),
                Type t when t == typeof(ushort) => DecodeUShort(encodedValue),
                _ => throw new NotSupportedException($"Type '{actualType}' is not supported for decoding.")
            };

            // Validate canonical encoding - prevents multiple IDs from resolving to the same value
            if (encodedValue != canonicalEncoding)
            {
                throw new ArgumentException(
                    $"Invalid non-canonical encoding '{encodedValue}'. The canonical encoding for this value is '{canonicalEncoding}'.",
                    nameof(encodedValue));
            }

            return decodedValue;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' to type {actualType.Name}.", nameof(encodedValue), ex);
        }
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeInt(string encodedValue)
    {
        var result = intEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = intEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeUInt(string encodedValue)
    {
        var result = uintEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = uintEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeLong(string encodedValue)
    {
        var result = longEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = longEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeULong(string encodedValue)
    {
        var result = ulongEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = ulongEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeShort(string encodedValue)
    {
        var result = shortEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = shortEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }

    private (object DecodedValue, string CanonicalEncoding) DecodeUShort(string encodedValue)
    {
        var result = ushortEncoder.Decode(encodedValue);
        if (result.Count == 0)
        {
            throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
        }

        var decodedValue = result[0];
        var canonicalEncoding = ushortEncoder.Encode(decodedValue);
        return (decodedValue, canonicalEncoding);
    }
}
