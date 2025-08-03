namespace CloakId.AspNetCore;

/// <summary>
/// Configuration options for CloakId ASP.NET Core integration.
/// </summary>
public class CloakIdAspNetCoreOptions
{
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
}
