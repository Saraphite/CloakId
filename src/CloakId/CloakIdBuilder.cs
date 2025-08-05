using CloakId.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

/// <summary>
/// A builder for configuring CloakId services.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CloakIdBuilder"/> class.
/// </remarks>
/// <param name="services">The service collection to add services to.</param>
public class CloakIdBuilder(IServiceCollection services) : ICloakIdBuilder
{
    /// <summary>
    /// Gets the service collection that CloakId services are being added to.
    /// </summary>
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Gets a value indicating whether a codec has already been configured for this builder.
    /// </summary>
    public bool HasCodecConfigured { get; private set; }

    /// <summary>
    /// Marks the builder as having a codec configured.
    /// </summary>
    public void MarkCodecConfigured()
    {
        HasCodecConfigured = true;
    }

    /// <summary>
    /// Builds the CloakId services and returns the configured CloakIdTypeInfoResolver.
    /// </summary>
    /// <param name="serviceProvider">The service provider to build from.</param>
    /// <returns>The configured CloakIdTypeInfoResolver.</returns>
    public CloakIdTypeInfoResolver Build(IServiceProvider serviceProvider)
    {
        var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
        return new CloakIdTypeInfoResolver(codec);
    }
}
