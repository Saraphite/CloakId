using CloakId;
using CloakId.Abstractions;
using CloakId.AspNetCore;
using CloakId.Sqids;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace CloakId.Tests;

public class ModelBindingTests
{
    private readonly ICloakIdCodec _codec;

    public ModelBindingTests()
    {
        var services = new ServiceCollection();
        services.AddCloakId().WithSqids();
        var serviceProvider = services.BuildServiceProvider();
        _codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
    }

    #region MvcBuilderExtensions Tests

    [Fact]
    public void AddCloakId_RegistersModelBinderProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Configure<MvcOptions>(options => { }); // Initialize MvcOptions
        var mvcBuilder = new TestMvcBuilder(services);

        // Act
        mvcBuilder.AddCloakIdModelBinding();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        Assert.Contains(mvcOptions.ModelBinderProviders, provider => provider is CloakIdModelBinderProvider);
        Assert.Equal(typeof(CloakIdModelBinderProvider), mvcOptions.ModelBinderProviders[0].GetType());
    }

    [Fact]
    public void AddCloakId_ReturnsBuilderForChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Configure<MvcOptions>(options => { });
        var mvcBuilder = new TestMvcBuilder(services);

        // Act
        var result = mvcBuilder.AddCloakIdModelBinding();

        // Assert
        Assert.Same(mvcBuilder, result);
    }

    #endregion

    #region CloakIdModelBinder Integration Tests

    [Fact]
    public void CloakIdModelBinder_CanBeCreated_WithCodec()
    {
        // Arrange
        var options = Options.Create(new CloakIdAspNetCoreOptions());

        // Act
        var binder = new CloakIdModelBinder(_codec, options);

        // Assert
        Assert.NotNull(binder);
    }

    [Fact]
    public void CloakIdModelBinder_Codec_CanEncodeAndDecode()
    {
        // Arrange
        var originalValue = 12345;

        // Act
        var encodedValue = _codec.Encode(originalValue, typeof(int));
        var decodedValue = _codec.Decode(encodedValue, typeof(int));

        // Assert
        Assert.Equal(originalValue, decodedValue);
        Assert.NotEqual(originalValue.ToString(), encodedValue); // Ensure it's actually encoded
    }

    [Fact]
    public void CloakIdModelBinder_Codec_HandlesDifferentNumericTypes()
    {
        // Test int
        var intValue = 123;
        var encodedInt = _codec.Encode(intValue, typeof(int));
        var decodedInt = _codec.Decode(encodedInt, typeof(int));
        Assert.Equal(intValue, decodedInt);

        // Test long
        var longValue = 98765432109876L;
        var encodedLong = _codec.Encode(longValue, typeof(long));
        var decodedLong = _codec.Decode(encodedLong, typeof(long));
        Assert.Equal(longValue, decodedLong);

        // Test uint
        var uintValue = 456u;
        var encodedUint = _codec.Encode(uintValue, typeof(uint));
        var decodedUint = _codec.Decode(encodedUint, typeof(uint));
        Assert.Equal(uintValue, decodedUint);
    }

    [Fact]
    public void CloakIdModelBinder_Codec_ThrowsForInvalidValue()
    {
        // Arrange
        var invalidEncodedValue = "not-a-valid-encoding";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _codec.Decode(invalidEncodedValue, typeof(int)));
    }

    #endregion

    #region Builder Pattern Tests

    [Fact]
    public void WithSqids_AfterWithRegisteredSqids_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithRegisteredSqids()
                .WithSqids());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    [Fact]
    public void WithRegisteredSqids_AfterWithSqids_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithSqids()
                .WithRegisteredSqids());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    [Fact]
    public void WithSqids_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithSqids()
                .WithSqids());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    [Fact]
    public void WithRegisteredSqids_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithRegisteredSqids()
                .WithRegisteredSqids());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    [Fact]
    public void WithCustomCodec_Generic_RegistersCustomCodec()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCloakId().WithCustomCodec<TestCustomCodec>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
        Assert.IsType<TestCustomCodec>(codec);
    }

    [Fact]
    public void WithCustomCodec_Instance_RegistersCustomCodec()
    {
        // Arrange
        var services = new ServiceCollection();
        var customCodec = new TestCustomCodec();

        // Act
        services.AddCloakId().WithCustomCodec(customCodec);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
        Assert.Same(customCodec, codec);
    }

    [Fact]
    public void WithCustomCodec_Factory_RegistersCustomCodec()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCloakId().WithCustomCodec(provider => new TestCustomCodec());

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
        Assert.IsType<TestCustomCodec>(codec);
    }

    [Fact]
    public void WithCustomCodec_AfterWithSqids_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithSqids()
                .WithCustomCodec<TestCustomCodec>());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    [Fact]
    public void WithSqids_AfterWithCustomCodec_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddCloakId()
                .WithCustomCodec<TestCustomCodec>()
                .WithSqids());

        Assert.Contains("A codec has already been configured", exception.Message);
    }

    #endregion

    #region Test Helpers

    private class TestMvcBuilder(IServiceCollection services) : IMvcBuilder
    {
        public IServiceCollection Services { get; } = services;
        public ApplicationPartManager PartManager { get; } = new ApplicationPartManager();
    }

    private class TestCustomCodec : ICloakIdCodec
    {
        public string Encode(object value, Type valueType)
        {
            return $"custom_{value}";
        }

        public object Decode(string encodedValue, Type targetType)
        {
            if (encodedValue.StartsWith("custom_") && encodedValue.Length > 7)
            {
                var valueString = encodedValue.Substring(7);
                return Convert.ChangeType(valueString, targetType);
            }
            throw new ArgumentException("Invalid encoded value");
        }
    }

    #endregion
}
