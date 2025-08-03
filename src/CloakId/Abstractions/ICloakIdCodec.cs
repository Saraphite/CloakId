namespace CloakId.Abstractions;

/// <summary>
/// Provides encoding and decoding functionality for numeric values.
/// </summary>
public interface ICloakIdCodec
{
    /// <summary>
    /// Encodes a numeric value to a string.
    /// </summary>
    /// <param name="value">The numeric value to encode.</param>
    /// <param name="valueType">The type of the numeric value.</param>
    /// <returns>The encoded string.</returns>
    string Encode(object value, Type valueType);

    /// <summary>
    /// Decodes a string back to a numeric value of the specified type.
    /// </summary>
    /// <param name="encodedValue">The encoded string.</param>
    /// <param name="targetType">The target numeric type.</param>
    /// <returns>The decoded numeric value.</returns>
    object Decode(string encodedValue, Type targetType);
}
