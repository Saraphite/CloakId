using CloakId.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

/// <summary>
/// Extension methods for configuring CloakId with custom codecs.
/// </summary>
public static class CloakIdBuilderExtensions
{
    /// <summary>
    /// Configures CloakId to use a custom codec implementation.
    /// </summary>
    /// <typeparam name="TCodec">The type of the custom codec that implements ICloakIdCodec.</typeparam>
    /// <param name="builder">The CloakId builder.</param>
    /// <returns>The updated CloakId builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a codec has already been configured.</exception>
    /// <remarks>
    /// When multiple ICloakIdCodec implementations are registered in the service container, 
    /// .NET's dependency injection returns the LAST registered service, which can lead to 
    /// unpredictable behavior. This validation prevents accidental multiple codec registrations.
    /// 
    /// If you need multiple codec support, consider implementing a composite codec pattern
    /// or use separate service scopes for different codec configurations.
    /// </remarks>
    public static ICloakIdBuilder WithCustomCodec<TCodec>(this ICloakIdBuilder builder)
        where TCodec : class, ICloakIdCodec
    {
        if (builder.HasCodecConfigured)
        {
            throw new InvalidOperationException(
                "A codec has already been configured for this CloakId builder. " +
                "You cannot call WithCustomCodec() after another codec configuration method has been called. " +
                "Multiple codec registrations can lead to unpredictable behavior since .NET DI returns the last registered service. " +
                "Use separate service scopes or implement a composite codec pattern if you need multiple codec support.");
        }

        // Register the custom codec as the ICloakIdCodec
        builder.Services.AddSingleton<ICloakIdCodec, TCodec>();

        // Mark codec as configured
        builder.MarkCodecConfigured();

        return builder;
    }

    /// <summary>
    /// Configures CloakId to use a custom codec instance.
    /// </summary>
    /// <param name="builder">The CloakId builder.</param>
    /// <param name="codecInstance">The custom codec instance that implements ICloakIdCodec.</param>
    /// <returns>The updated CloakId builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a codec has already been configured.</exception>
    /// <remarks>
    /// When multiple ICloakIdCodec implementations are registered in the service container, 
    /// .NET's dependency injection returns the LAST registered service, which can lead to 
    /// unpredictable behavior. This validation prevents accidental multiple codec registrations.
    /// </remarks>
    public static ICloakIdBuilder WithCustomCodec(this ICloakIdBuilder builder, ICloakIdCodec codecInstance)
    {
        if (builder.HasCodecConfigured)
        {
            throw new InvalidOperationException(
                "A codec has already been configured for this CloakId builder. " +
                "You cannot call WithCustomCodec() after another codec configuration method has been called. " +
                "Multiple codec registrations can lead to unpredictable behavior since .NET DI returns the last registered service.");
        }

        // Register the custom codec instance as the ICloakIdCodec
        builder.Services.AddSingleton(codecInstance);

        // Mark codec as configured
        builder.MarkCodecConfigured();

        return builder;
    }

    /// <summary>
    /// Configures CloakId to use a custom codec with a factory method.
    /// </summary>
    /// <param name="builder">The CloakId builder.</param>
    /// <param name="codecFactory">A factory method that creates the custom codec instance.</param>
    /// <returns>The updated CloakId builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a codec has already been configured.</exception>
    /// <remarks>
    /// When multiple ICloakIdCodec implementations are registered in the service container, 
    /// .NET's dependency injection returns the LAST registered service, which can lead to 
    /// unpredictable behavior. This validation prevents accidental multiple codec registrations.
    /// </remarks>
    public static ICloakIdBuilder WithCustomCodec(this ICloakIdBuilder builder, Func<IServiceProvider, ICloakIdCodec> codecFactory)
    {
        if (builder.HasCodecConfigured)
        {
            throw new InvalidOperationException(
                "A codec has already been configured for this CloakId builder. " +
                "You cannot call WithCustomCodec() after another codec configuration method has been called. " +
                "Multiple codec registrations can lead to unpredictable behavior since .NET DI returns the last registered service.");
        }

        // Register the custom codec factory as the ICloakIdCodec
        builder.Services.AddSingleton(codecFactory);

        // Mark codec as configured
        builder.MarkCodecConfigured();

        return builder;
    }
}
