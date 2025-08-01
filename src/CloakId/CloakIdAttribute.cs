using System;

namespace CloakId;

/// <summary>
/// Marks a numeric property to be encoded/decoded as a cloaked string during JSON serialization.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CloakIdAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the minimum length of the encoded string.
    /// If not specified, uses the codec's default minimum length.
    /// </summary>
    public int MinLength { get; set; } = 0;

    /// <summary>
    /// Gets or sets a custom alphabet to use for encoding this property.
    /// If not specified, uses the codec's default alphabet.
    /// </summary>
    public string? Alphabet { get; set; }
}
