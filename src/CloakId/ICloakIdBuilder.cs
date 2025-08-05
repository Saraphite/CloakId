using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

/// <summary>
/// A builder interface for configuring CloakId services.
/// </summary>
public interface ICloakIdBuilder
{
    /// <summary>
    /// Gets the service collection that CloakId services are being added to.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets a value indicating whether a codec has already been configured for this builder.
    /// </summary>
    bool HasCodecConfigured { get; }

    /// <summary>
    /// Marks the builder as having a codec configured.
    /// </summary>
    void MarkCodecConfigured();

    /// <summary>
    /// Builds the CloakId services and returns the configured service provider factory.
    /// </summary>
    /// <param name="serviceProvider">The service provider to build from.</param>
    /// <returns>The configured CloakIdTypeInfoResolver.</returns>
    CloakIdTypeInfoResolver Build(IServiceProvider serviceProvider);
}
