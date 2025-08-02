using CloakId.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CloakId.AspNetCore;

/// <summary>
/// Model binder for automatically converting encoded CloakId strings to their underlying numeric types.
/// </summary>
public class CloakIdModelBinder(ICloakIdCodec codec) : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

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
            // If decoding fails, let the default model binder handle it
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
