using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CloakId.AspNetCore;

/// <summary>
/// Extension methods for configuring CloakId OpenAPI support.
/// </summary>
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Adds CloakId OpenAPI filter to ensure parameters marked with [CloakId] attribute
    /// are correctly documented as string parameters in the OpenAPI specification.
    /// </summary>
    /// <param name="options">The SwaggerGenOptions to configure.</param>
    /// <returns>The SwaggerGenOptions for chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddSwaggerGen(c =>
    /// {
    ///     c.AddCloakIdSupport();
    /// });
    /// </code>
    /// </example>
    public static SwaggerGenOptions AddCloakIdSupport(this SwaggerGenOptions options)
    {
        // The correct method for adding operation filters in Swashbuckle
        options.OperationFilter<CloakIdOpenApiFilter>();
        return options;
    }
}
