using CloakId.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Sqids;

namespace CloakId.Sqids;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CloakId services with Sqids encoding to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Optional action to configure CloakId options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCloakIdWithSqids(
        this IServiceCollection services,
        Action<CloakIdOptions>? configureOptions = null)
    {
        // Configure options
        var options = new CloakIdOptions();
        configureOptions?.Invoke(options);

        // Register Sqids encoders for different numeric types
        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<int>(sqidsOptions);
        });

        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<uint>(sqidsOptions);
        });

        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<long>(sqidsOptions);
        });

        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<ulong>(sqidsOptions);
        });

        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<short>(sqidsOptions);
        });

        services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<ushort>(sqidsOptions);
        });

        // Register the SqidsCodec as the ICloakIdCodec
        services.AddSingleton<ICloakIdCodec, SqidsCodec>();

        return services;
    }
}
