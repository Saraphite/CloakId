using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId.AspNetCore;

/// <summary>
/// Model binder provider that creates CloakIdModelBinder instances for numeric parameters.
/// This version provides binding for all numeric types and attempts to decode CloakId strings,
/// falling back to default binding if decoding fails.
/// </summary>
public class CloakIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.ModelType;
        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;

        // Only provide binder for numeric types that CloakId supports
        if (IsNumericType(underlyingType))
        {
            return new CloakIdModelBinder(context.Services.GetRequiredService<Abstractions.ICloakIdCodec>());
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
