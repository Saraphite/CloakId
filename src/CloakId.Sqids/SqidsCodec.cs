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
            object decodedValue;
            
            if (actualType == typeof(int))
            {
                var result = intEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else if (actualType == typeof(uint))
            {
                var result = uintEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else if (actualType == typeof(long))
            {
                var result = longEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else if (actualType == typeof(ulong))
            {
                var result = ulongEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else if (actualType == typeof(short))
            {
                var result = shortEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else if (actualType == typeof(ushort))
            {
                var result = ushortEncoder.Decode(encodedValue);
                if (result.Count == 0)
                    throw new ArgumentException($"Unable to decode '{encodedValue}' - invalid format.", nameof(encodedValue));
                decodedValue = result[0];
            }
            else
            {
                throw new NotSupportedException($"Type '{actualType}' is not supported for decoding.");
            }

            // Validate canonical encoding - re-encode the decoded value and ensure it matches the input
            // This prevents multiple IDs from resolving to the same value (e.g., "2fs" and "OSc" both decode to 3168)
            string canonicalEncoding;
            
            if (actualType == typeof(int))
            {
                canonicalEncoding = intEncoder.Encode((int)decodedValue);
            }
            else if (actualType == typeof(uint))
            {
                canonicalEncoding = uintEncoder.Encode((uint)decodedValue);
            }
            else if (actualType == typeof(long))
            {
                canonicalEncoding = longEncoder.Encode((long)decodedValue);
            }
            else if (actualType == typeof(ulong))
            {
                canonicalEncoding = ulongEncoder.Encode((ulong)decodedValue);
            }
            else if (actualType == typeof(short))
            {
                canonicalEncoding = shortEncoder.Encode((short)decodedValue);
            }
            else if (actualType == typeof(ushort))
            {
                canonicalEncoding = ushortEncoder.Encode((ushort)decodedValue);
            }
            else
            {
                throw new NotSupportedException($"Type '{actualType}' is not supported for canonical validation.");
            }

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
}
