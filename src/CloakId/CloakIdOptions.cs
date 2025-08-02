namespace CloakId;

/// <summary>
/// Base configuration options for CloakId.
/// </summary>
public class CloakIdOptions
{
    /// <summary>
    /// Gets or sets the minimum length of generated encoded IDs.
    /// Default is 0 (no minimum length).
    /// </summary>
    public int MinLength { get; set; } = 0;

    /// <summary>
    /// Gets or sets the custom alphabet/character set for ID encoding.
    /// If null, the default alphabet for the specific implementation will be used.
    /// Most implementations support customizing the characters used in encoded IDs.
    /// </summary>
    public string? Alphabet { get; set; }
}
