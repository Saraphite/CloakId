using CloakId.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CloakId services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCloakId(this IServiceCollection services)
    {
        // Register the type info resolver for handling [Cloak] attributes
        services.AddSingleton(provider =>
        {
            var codec = provider.GetRequiredService<ICloakIdCodec>();
            return new CloakIdTypeInfoResolver(codec);
        });

        return services;
    }
}
