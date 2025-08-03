using System.Reflection;
using System.Text.Json;
using CloakId.AspNetCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace CloakId.Tests;

public class CloakIdOpenApiFilterTests
{
    [Fact]
    public void Apply_WithCloakIdParameter_ModifiesParameterToString()
    {
        // Arrange
        var filter = new CloakIdOpenApiFilter();
        var operation = new OpenApiOperation
        {
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "id",
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                }
            ]
        };

        var methodInfo = GetType().GetMethod(nameof(TestMethod), BindingFlags.NonPublic | BindingFlags.Static)!;
        var context = new TestOperationFilterContext(methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        var parameter = operation.Parameters.First();
        Assert.Equal("string", parameter.Schema.Type);
        Assert.Null(parameter.Schema.Format);
        Assert.Contains("Encoded string representation", parameter.Schema.Description);
        Assert.True(parameter.Extensions.ContainsKey("x-cloakid"));
        Assert.Equal("int", ((Microsoft.OpenApi.Any.OpenApiString)parameter.Extensions["x-cloakid-original-type"]).Value);
    }

    [Fact]
    public void Apply_WithNonCloakIdParameter_DoesNotModifyParameter()
    {
        // Arrange
        var filter = new CloakIdOpenApiFilter();
        var operation = new OpenApiOperation
        {
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "regularId",
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                }
            ]
        };

        var methodInfo = GetType().GetMethod(nameof(TestMethodWithoutCloakId), BindingFlags.NonPublic | BindingFlags.Static)!;
        var context = new TestOperationFilterContext(methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        var parameter = operation.Parameters.First();
        Assert.Equal("integer", parameter.Schema.Type);
        Assert.Equal("int32", parameter.Schema.Format);
        Assert.False(parameter.Extensions.ContainsKey("x-cloakid"));
    }

    [Fact]
    public void Apply_WithStringParameter_DoesNotModifyParameter()
    {
        // Arrange
        var filter = new CloakIdOpenApiFilter();
        var operation = new OpenApiOperation
        {
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "name",
                    Schema = new OpenApiSchema { Type = "string" }
                }
            ]
        };

        var methodInfo = GetType().GetMethod(nameof(TestMethodWithString), BindingFlags.NonPublic | BindingFlags.Static)!;
        var context = new TestOperationFilterContext(methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        var parameter = operation.Parameters.First();
        Assert.Equal("string", parameter.Schema.Type);
        Assert.False(parameter.Extensions.ContainsKey("x-cloakid"));
    }

    [Fact]
    public void Apply_WithNullableIntParameter_ModifiesParameterToString()
    {
        // Arrange
        var filter = new CloakIdOpenApiFilter();
        var operation = new OpenApiOperation
        {
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "optionalId",
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32", Nullable = true }
                }
            ]
        };

        var methodInfo = GetType().GetMethod(nameof(TestMethodWithNullableInt), BindingFlags.NonPublic | BindingFlags.Static)!;
        var context = new TestOperationFilterContext(methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        var parameter = operation.Parameters.First();
        Assert.Equal("string", parameter.Schema.Type);
        Assert.Contains("int?", ((Microsoft.OpenApi.Any.OpenApiString)parameter.Extensions["x-cloakid-original-type"]).Value);
    }

    // Test methods for reflection
    private static void TestMethod([CloakId] int id) { }
    private static void TestMethodWithoutCloakId(int regularId) { }
    private static void TestMethodWithString([CloakId] string name) { }
    private static void TestMethodWithNullableInt([CloakId] int? optionalId) { }

    // Helper class for testing
    private class TestOperationFilterContext(MethodInfo methodInfo) : OperationFilterContext(
            new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription(),
            new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new JsonSerializerOptions())),
            new SchemaRepository(),
            methodInfo);
}
