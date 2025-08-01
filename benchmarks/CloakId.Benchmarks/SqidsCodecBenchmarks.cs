using BenchmarkDotNet.Attributes;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;

namespace CloakId.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class SqidsCodecBenchmarks
{
    private ICloakIdCodec _codec = null!;
    private const int TestValue = 123456;
    private const uint TestUintValue = 987654u;
    private const long TestLongValue = 123456789012345L;
    private const string ValidEncodedValue = "A6das1"; // Pre-encoded value for 123456
    private const string InvalidEncodedValue = "InvalidValue";

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddCloakIdWithSqids();
        var serviceProvider = services.BuildServiceProvider();
        _codec = serviceProvider.GetRequiredService<ICloakIdCodec>();
    }

    [Benchmark]
    public string EncodeInt32_HappyPath()
    {
        return _codec.Encode(TestValue, typeof(int));
    }

    [Benchmark]
    public string EncodeUInt32_HappyPath()
    {
        return _codec.Encode(TestUintValue, typeof(uint));
    }

    [Benchmark]
    public string EncodeLong_HappyPath()
    {
        return _codec.Encode(TestLongValue, typeof(long));
    }

    [Benchmark]
    public string EncodeNullableInt_HappyPath()
    {
        return _codec.Encode(TestValue, typeof(int?));
    }

    [Benchmark]
    public object DecodeInt32_HappyPath()
    {
        return _codec.Decode(ValidEncodedValue, typeof(int));
    }

    [Benchmark]
    public object DecodeNullableInt_HappyPath()
    {
        return _codec.Decode(ValidEncodedValue, typeof(int?));
    }

    [Benchmark]
    public string EncodeDecodeRoundTrip_Int32()
    {
        var encoded = _codec.Encode(TestValue, typeof(int));
        var decoded = _codec.Decode(encoded, typeof(int));
        return encoded;
    }

    [Benchmark]
    public bool TryEncodeUnsupportedType_SadPath()
    {
        try
        {
            _codec.Encode("not a number", typeof(string));
            return true;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    [Benchmark]
    public bool TryDecodeInvalidValue_SadPath()
    {
        try
        {
            _codec.Decode(InvalidEncodedValue, typeof(int));
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    [Benchmark]
    public bool TryDecodeEmptyString_SadPath()
    {
        try
        {
            _codec.Decode("", typeof(int));
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    [Benchmark]
    public bool TryDecodeNullString_SadPath()
    {
        try
        {
            _codec.Decode(null!, typeof(int));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
