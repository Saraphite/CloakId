using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure CloakId services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CloakId services to the service collection using a fluent builder pattern.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>A builder for configuring CloakId services.</returns>
    public static ICloakIdBuilder AddCloakId(this IServiceCollection services)
    {
        var builder = new CloakIdBuilder(services);

        // Register the type info resolver for handling [Cloak] attributes
        services.AddSingleton(builder.Build);

        return builder;
    }
}
