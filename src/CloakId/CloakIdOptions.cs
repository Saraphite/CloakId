namespace CloakId;

/// <summary>
/// Base configuration options for CloakId.
/// </summary>
public class CloakIdOptions
{
    private int _minLength = 0;
    private string? _alphabet;

    /// <summary>
    /// Gets or sets the minimum length of generated encoded IDs.
    /// Default is 0 (no minimum length).
    /// </summary>
    public int MinLength 
    { 
        get => _minLength; 
        set 
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "MinLength must be 0 or greater.");
            _minLength = value;
        }
    }

    /// <summary>
    /// Gets or sets the custom alphabet/character set for ID encoding.
    /// If null, the default alphabet for the specific implementation will be used.
    /// Most implementations support customizing the characters used in encoded IDs.
    /// </summary>
    public string? Alphabet 
    { 
        get => _alphabet; 
        set 
        {
            if (value != null)
            {
                ValidateAlphabet(value);
            }
            _alphabet = value;
        }
    }

    /// <summary>
    /// Gets or sets whether model binding should fall back to parsing numeric values
    /// when CloakId decoding fails. Default is false for better security.
    /// 
    /// Security Note: Setting this to false provides better security by rejecting
    /// any non-encoded values, but may break existing clients that send numeric IDs.
    /// Setting this to true allows fallback to numeric parsing but could potentially
    /// expose alphabet patterns through systematic testing.
    /// </summary>
    public bool AllowNumericFallback { get; set; } = false;

    /// <summary>
    /// Validates the provided alphabet according to common encoding requirements.
    /// </summary>
    /// <param name="alphabet">The alphabet to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the alphabet is invalid.</exception>
    private static void ValidateAlphabet(string alphabet)
    {
        if (string.IsNullOrWhiteSpace(alphabet))
            throw new ArgumentException("Alphabet cannot be null, empty, or whitespace.", nameof(alphabet));

        if (alphabet.Length < 3)
            throw new ArgumentException("Alphabet must contain at least 3 characters.", nameof(alphabet));

        // Check for duplicate characters
        var uniqueChars = new HashSet<char>();
        foreach (char c in alphabet)
        {
            if (!uniqueChars.Add(c))
                throw new ArgumentException($"Alphabet contains duplicate character '{c}'. All characters must be unique.", nameof(alphabet));
        }

        // Check for whitespace characters (which can cause issues in URLs and other contexts)
        foreach (char c in alphabet)
        {
            if (char.IsWhiteSpace(c))
                throw new ArgumentException($"Alphabet contains whitespace character at position {alphabet.IndexOf(c)}. Whitespace characters are not allowed.", nameof(alphabet));
        }
    }
}
