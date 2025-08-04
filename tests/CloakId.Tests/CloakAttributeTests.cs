using System.Text.Json;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CloakId.Tests;

public class CloakAttributeTests
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ICloakIdCodec _codec;

    public CloakAttributeTests()
    {
        // Set up dependency injection
        var services = new ServiceCollection();
        services.AddCloakIdWithSqids();
        services.AddCloakId();

        var serviceProvider = services.BuildServiceProvider();

        _codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
        var typeInfoResolver = serviceProvider.GetRequiredService<CloakIdTypeInfoResolver>();

        _jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = typeInfoResolver
        };
    }

    [Fact]
    public void SerializeDeserialize_CloakAttribute_EncodesAndDecodesCorrectly()
    {
        // Arrange
        var dto = new TestDto
        {
            CloakedId = 12345,
            RegularId = 67890,
            Name = "Test"
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<TestDto>(json, _jsonOptions);

        // Assert
        Assert.Contains("\"CloakedId\":", json);
        Assert.Contains("\"RegularId\":67890", json); // Regular ID should remain numeric
        Assert.DoesNotContain("12345", json); // Cloaked ID should be encoded

        Assert.Equal(12345, deserialized!.CloakedId);
        Assert.Equal(67890, deserialized.RegularId);
        Assert.Equal("Test", deserialized.Name);
    }

    [Fact]
    public void SerializeDeserialize_DifferentNumericTypes_WorksCorrectly()
    {
        // Arrange
        var dto = new MultiTypeTestDto
        {
            IntId = 123,
            LongId = 456789012345L,
            UIntId = 789U,
            ShortId = 321
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<MultiTypeTestDto>(json, _jsonOptions);

        // Assert
        Assert.Equal(123, deserialized!.IntId);
        Assert.Equal(456789012345L, deserialized.LongId);
        Assert.Equal(789U, deserialized.UIntId);
        Assert.Equal((short)321, deserialized.ShortId);

        // Verify all values are encoded as strings in JSON
        Assert.DoesNotContain("123", json);
        Assert.DoesNotContain("456789012345", json);
        Assert.DoesNotContain("789", json);
        Assert.DoesNotContain("321", json);
    }

    [Fact]
    public void SerializeDeserialize_NullableTypes_HandlesNullCorrectly()
    {
        // Arrange
        var dto = new NullableTestDto
        {
            RequiredId = 42,
            OptionalId = null
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<NullableTestDto>(json, _jsonOptions);

        // Assert
        Assert.Equal(42, deserialized!.RequiredId);
        Assert.Null(deserialized.OptionalId);
        Assert.Contains("\"OptionalId\":null", json);
    }

    [Fact]
    public void SerializeDeserialize_NullableTypes_HandlesValueCorrectly()
    {
        // Arrange
        var dto = new NullableTestDto
        {
            RequiredId = 42,
            OptionalId = 99
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<NullableTestDto>(json, _jsonOptions);

        // Assert
        Assert.Equal(42, deserialized!.RequiredId);
        Assert.Equal(99, deserialized.OptionalId);
        Assert.DoesNotContain("99", json); // Should be encoded
    }

    [Fact]
    public void DirectCodec_EncodeDecodeRoundTrip_PreservesValue()
    {
        // Arrange
        var originalValue = 98765;

        // Act
        var encoded = _codec.Encode(originalValue, typeof(int));
        var decoded = (int)_codec.Decode(encoded, typeof(int));

        // Assert
        Assert.Equal(originalValue, decoded);
        Assert.NotEqual(originalValue.ToString(), encoded); // Should be encoded
    }

    private class TestDto
    {
        [Cloak]
        public int CloakedId { get; set; }

        public int RegularId { get; set; }

        public string Name { get; set; } = null!;
    }

    private class MultiTypeTestDto
    {
        [Cloak] public int IntId { get; set; }
        [Cloak] public long LongId { get; set; }
        [Cloak] public uint UIntId { get; set; }
        [Cloak] public short ShortId { get; set; }
    }

    private class NullableTestDto
    {
        [Cloak]
        public int RequiredId { get; set; }

        [Cloak]
        public int? OptionalId { get; set; }
    }
}
