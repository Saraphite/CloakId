using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CloakId.AspNetCore;

/// <summary>
/// OpenAPI operation filter that modifies parameter schemas for CloakId-marked parameters.
/// This ensures that the API documentation correctly shows string parameters instead of numeric types
/// for parameters marked with the [Cloak] attribute.
/// </summary>
public class CloakIdOpenApiFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) return;

        // Get the method info from the context
        var methodInfo = context.MethodInfo;
        var parameters = methodInfo.GetParameters();

        for (int i = 0; i < parameters.Length && i < operation.Parameters.Count; i++)
        {
            var parameterInfo = parameters[i];
            var openApiParameter = operation.Parameters.FirstOrDefault(p =>
                string.Equals(p.Name, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (openApiParameter == null) continue;

            // Check if the parameter has the Cloak attribute
            var cloakAttribute = parameterInfo.GetCustomAttribute<CloakAttribute>();
            if (cloakAttribute != null && IsNumericType(parameterInfo.ParameterType))
            {
                // Modify the parameter to be a string type in the OpenAPI spec
                openApiParameter.Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = null, // Remove any numeric format
                    Description = openApiParameter.Schema?.Description ??
                        $"Encoded string representation of a {GetFriendlyTypeName(parameterInfo.ParameterType)} value. " +
                        "This parameter accepts encoded string values (e.g., 'A6das1') rather than raw numeric values."
                };

                // Add example if the original had one
                if (openApiParameter.Example != null)
                {
                    openApiParameter.Example = new Microsoft.OpenApi.Any.OpenApiString("A6das1");
                }

                // Update extensions to indicate this is a CloakId parameter
                openApiParameter.Extensions["x-cloakid"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true);
                openApiParameter.Extensions["x-cloakid-original-type"] =
                    new Microsoft.OpenApi.Any.OpenApiString(GetFriendlyTypeName(parameterInfo.ParameterType));
            }
        }
    }

    private static bool IsNumericType(Type type)
    {
        var actualType = Nullable.GetUnderlyingType(type) ?? type;
        return actualType == typeof(int) ||
               actualType == typeof(uint) ||
               actualType == typeof(long) ||
               actualType == typeof(ulong) ||
               actualType == typeof(short) ||
               actualType == typeof(ushort);
    }

    private static string GetFriendlyTypeName(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return GetSimpleTypeName(underlyingType) + "?";
        }
        return GetSimpleTypeName(type);
    }

    private static string GetSimpleTypeName(Type type)
    {
        return type.Name switch
        {
            "Int32" => "int",
            "UInt32" => "uint",
            "Int64" => "long",
            "UInt64" => "ulong",
            "Int16" => "short",
            "UInt16" => "ushort",
            _ => type.Name.ToLowerInvariant()
        };
    }
}
