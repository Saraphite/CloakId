using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId;

/// <summary>
/// Extension methods for configuring CloakId model binding in ASP.NET Core.
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Adds CloakId model binding support to ASP.NET Core MVC.
    /// This enables automatic conversion of encoded route parameters to their underlying numeric types
    /// for parameters marked with the [CloakId] attribute.
    /// </summary>
    /// <param name="builder">The IMvcBuilder to configure.</param>
    /// <returns>The IMvcBuilder for chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddControllers().AddCloakIdModelBinding();
    /// </code>
    /// </example>
    public static IMvcBuilder AddCloakIdModelBinding(this IMvcBuilder builder)
    {
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new CloakIdModelBinderProvider());
        });

        return builder;
    }
}
