using CloakId.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace CloakId.AspNetCore;

/// <summary>
/// Model binder for automatically converting encoded CloakId strings to their underlying numeric types.
/// </summary>
public class CloakIdModelBinder(ICloakIdCodec codec, IOptions<CloakIdAspNetCoreOptions> options) : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        // Check if the property has the CloakId attribute
        var modelType = bindingContext.ModelType;
        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;

        // For route parameters, we need to check if this is a numeric type that could be cloaked
        if (!IsNumericType(underlyingType)) return Task.CompletedTask;

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (value == ValueProviderResult.None) return Task.CompletedTask;

        var stringValue = value.FirstValue;
        if (string.IsNullOrEmpty(stringValue)) return Task.CompletedTask;

        try
        {
            var decodedValue = codec.Decode(stringValue, underlyingType);
            bindingContext.Result = ModelBindingResult.Success(decodedValue);
        }
        catch (Exception)
        {
            // Check if numeric fallback is allowed
            if (!options.Value.AllowNumericFallback)
            {
                // If fallback is disabled, fail the binding with a clear error
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                    $"Invalid encoded value '{stringValue}'. Numeric fallback is disabled.");
                return Task.CompletedTask;
            }

            // If decoding fails and fallback is allowed, let the default model binder handle it
            // This allows for backwards compatibility with numeric IDs
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(int) ||
               type == typeof(uint) ||
               type == typeof(long) ||
               type == typeof(ulong) ||
               type == typeof(short) ||
               type == typeof(ushort);
    }
}
