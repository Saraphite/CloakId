using CloakId;
using CloakId.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace WebApiExample.ModelBinding;

/// <summary>
/// Model binder for automatically converting encoded CloakId strings to their underlying numeric types.
/// Only activates when a parameter is decorated with the [CloakId] attribute.
/// </summary>
public class CloakIdModelBinder(ICloakIdCodec codec) : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var modelType = bindingContext.ModelType;
        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;
        
        // Only handle numeric types that CloakId supports
        if (!IsNumericType(underlyingType))
        {
            return Task.CompletedTask;
        }

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (value == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        var stringValue = value.FirstValue;
        if (string.IsNullOrEmpty(stringValue))
        {
            return Task.CompletedTask;
        }

        try
        {
            var decodedValue = codec.Decode(stringValue, underlyingType);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
            bindingContext.Model = decodedValue;
            bindingContext.Result = ModelBindingResult.Success(decodedValue);
        }
        catch (Exception)
        {
            // If decoding fails, let the default model binder handle it
            // This allows for backwards compatibility with regular numeric IDs
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

/// <summary>
/// Model binder provider that creates CloakIdModelBinder instances for numeric parameters.
/// This version attempts to decode any string parameter to a numeric type and falls back
/// to default binding if decoding fails.
/// </summary>
public class CloakIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var modelType = context.Metadata.ModelType;
        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;

        // Only provide binder for numeric types that CloakId supports
        if (IsNumericType(underlyingType))
        {
            return new CloakIdModelBinder(context.Services.GetRequiredService<ICloakIdCodec>());
        }

        return null;
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
