using System.Diagnostics.Metrics;
using System.Reflection;

namespace CloakId.AspNetCore;

/// <summary>
/// Provides metrics for monitoring CloakId model binding behavior.
/// </summary>
public class CloakIdMetrics
{
    private static readonly Meter Meter = new("CloakId.AspNetCore", GetAssemblyVersion());

    /// <summary>
    /// Gets the version of the current assembly for use in metrics.
    /// </summary>
    private static string GetAssemblyVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version?.ToString() ?? "1.0.0";
    }

    /// <summary>
    /// Counter for successful CloakId decodings during model binding.
    /// </summary>
    public static readonly Counter<long> DecodingSuccessCounter = Meter.CreateCounter<long>(
        "cloakid_model_binding_decoding_success_total",
        "count",
        "Total number of successful CloakId decodings during model binding");

    /// <summary>
    /// Counter for failed CloakId decodings during model binding.
    /// </summary>
    public static readonly Counter<long> DecodingFailureCounter = Meter.CreateCounter<long>(
        "cloakid_model_binding_decoding_failure_total",
        "count",
        "Total number of failed CloakId decodings during model binding");

    /// <summary>
    /// Counter for numeric fallback usage during model binding.
    /// This is particularly important for security monitoring.
    /// </summary>
    public static readonly Counter<long> NumericFallbackCounter = Meter.CreateCounter<long>(
        "cloakid_model_binding_numeric_fallback_total",
        "count",
        "Total number of times numeric fallback was used during model binding");

    /// <summary>
    /// Counter for rejected requests when numeric fallback is disabled.
    /// </summary>
    public static readonly Counter<long> FallbackRejectionCounter = Meter.CreateCounter<long>(
        "cloakid_model_binding_fallback_rejection_total",
        "count",
        "Total number of requests rejected when numeric fallback is disabled");

    /// <summary>
    /// Histogram for CloakId decoding duration during model binding.
    /// </summary>
    public static readonly Histogram<double> DecodingDuration = Meter.CreateHistogram<double>(
        "cloakid_model_binding_decoding_duration_ms",
        "milliseconds",
        "Duration of CloakId decoding operations during model binding");
}
