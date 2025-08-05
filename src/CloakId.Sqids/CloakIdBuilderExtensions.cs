using CloakId.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Sqids;

namespace CloakId.Sqids;

/// <summary>
/// Extension methods for configuring CloakId with Sqids encoding.
/// </summary>
public static class CloakIdBuilderExtensions
{
    /// <summary>
    /// Configures CloakId to use Sqids encoding with the specified options.
    /// </summary>
    /// <param name="builder">The CloakId builder.</param>
    /// <param name="configureOptions">Optional action to configure CloakId options.</param>
    /// <returns>The updated CloakId builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a codec has already been configured.</exception>
    /// <remarks>
    /// This method prevents multiple codec registrations to avoid unpredictable behavior.
    /// When multiple ICloakIdCodec implementations are registered, .NET DI returns the last registered service.
    /// </remarks>
    public static ICloakIdBuilder WithSqids(
        this ICloakIdBuilder builder,
        Action<CloakIdOptions>? configureOptions = null)
    {
        if (builder.HasCodecConfigured)
        {
            throw new InvalidOperationException(
                "A codec has already been configured for this CloakId builder. " +
                "You cannot call WithSqids() after another codec configuration method has been called. " +
                "Multiple codec registrations can lead to unpredictable behavior since .NET DI returns the last registered service.");
        }

        // Configure options
        var options = new CloakIdOptions();
        configureOptions?.Invoke(options);

        // Register Sqids encoders for different numeric types
        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<int>(sqidsOptions);
        });

        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<uint>(sqidsOptions);
        });

        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<long>(sqidsOptions);
        });

        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<ulong>(sqidsOptions);
        });

        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<short>(sqidsOptions);
        });

        builder.Services.AddSingleton(provider =>
        {
            var sqidsOptions = new SqidsOptions
            {
                MinLength = options.MinLength
            };
            if (options.Alphabet != null) sqidsOptions.Alphabet = options.Alphabet;
            return new SqidsEncoder<ushort>(sqidsOptions);
        });

        // Register the SqidsCodec as the ICloakIdCodec
        builder.Services.AddSingleton<ICloakIdCodec, SqidsCodec>();

        // Mark codec as configured
        builder.MarkCodecConfigured();

        return builder;
    }

    /// <summary>
    /// Configures CloakId to use a previously registered Sqids setup.
    /// Use this when you already have Sqids encoders registered in your DI container.
    /// </summary>
    /// <param name="builder">The CloakId builder.</param>
    /// <returns>The updated CloakId builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a codec has already been configured.</exception>
    /// <remarks>
    /// This method prevents multiple codec registrations to avoid unpredictable behavior.
    /// When multiple ICloakIdCodec implementations are registered, .NET DI returns the last registered service.
    /// </remarks>
    public static ICloakIdBuilder WithRegisteredSqids(this ICloakIdBuilder builder)
    {
        if (builder.HasCodecConfigured)
        {
            throw new InvalidOperationException(
                "A codec has already been configured for this CloakId builder. " +
                "You cannot call WithRegisteredSqids() after another codec configuration method has been called. " +
                "Multiple codec registrations can lead to unpredictable behavior since .NET DI returns the last registered service.");
        }

        // Just register the SqidsCodec - assumes encoders are already registered
        builder.Services.AddSingleton<ICloakIdCodec, SqidsCodec>();

        // Mark codec as configured
        builder.MarkCodecConfigured();

        return builder;
    }
}
