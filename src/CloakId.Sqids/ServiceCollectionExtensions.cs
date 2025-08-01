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
    /// <param name="alphabet">Optional custom alphabet for Sqids. If null, default alphabet will be used.</param>
    /// <param name="minLength">Minimum length of generated IDs.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCloakIdWithSqids(
        this IServiceCollection services,
        string? alphabet = null,
        int minLength = 0)
    {
        // Register Sqids encoders for different numeric types
        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<int>(options);
        });

        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<uint>(options);
        });

        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<long>(options);
        });

        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<ulong>(options);
        });

        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<short>(options);
        });

        services.AddSingleton(provider =>
        {
            var options = new SqidsOptions
            {
                MinLength = minLength
            };
            if (alphabet != null) options.Alphabet = alphabet;
            return new SqidsEncoder<ushort>(options);
        });

        // Register the SqidsCodec as the ICloakIdCodec
        services.AddSingleton<ICloakIdCodec, SqidsCodec>();

        return services;
    }
}
