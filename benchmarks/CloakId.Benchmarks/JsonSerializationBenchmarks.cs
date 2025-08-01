using BenchmarkDotNet.Attributes;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace CloakId.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class JsonSerializationBenchmarks
{
    private JsonSerializerOptions _options = null!;
    private TestModel _testModel = null!;
    private string _validJson = null!;
    private string _invalidJson = null!;

    public class TestModel
    {
        [CloakId]
        public int UserId { get; set; }

        [CloakId]
        public long AccountId { get; set; }

        [CloakId]
        public int? OptionalId { get; set; }

        public string Name { get; set; } = "Test";

        public int RegularId { get; set; }
    }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddCloakIdWithSqids();
        var serviceProvider = services.BuildServiceProvider();
        var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();

        _options = new JsonSerializerOptions
        {
            TypeInfoResolver = new CloakIdTypeInfoResolver(codec)
        };

        _testModel = new TestModel
        {
            UserId = 123456,
            AccountId = 987654321012345L,
            OptionalId = 999,
            Name = "John Doe",
            RegularId = 42
        };

        // Pre-serialize for deserialization benchmarks
        _validJson = JsonSerializer.Serialize(_testModel, _options);
        _invalidJson = """{"UserId": "InvalidEncodedValue", "AccountId": "AnotherInvalid", "Name": "Test", "RegularId": 42}""";
    }

    [Benchmark]
    public string SerializeModel_HappyPath()
    {
        return JsonSerializer.Serialize(_testModel, _options);
    }

    [Benchmark]
    public TestModel? DeserializeModel_HappyPath()
    {
        return JsonSerializer.Deserialize<TestModel>(_validJson, _options);
    }

    [Benchmark]
    public string SerializeDeserializeRoundTrip()
    {
        var json = JsonSerializer.Serialize(_testModel, _options);
        var deserialized = JsonSerializer.Deserialize<TestModel>(json, _options);
        return json;
    }

    [Benchmark]
    public string SerializeModelWithNulls()
    {
        var modelWithNulls = new TestModel
        {
            UserId = 123,
            AccountId = 456,
            OptionalId = null, // This should be handled properly
            Name = "Null Test"
        };
        return JsonSerializer.Serialize(modelWithNulls, _options);
    }

    [Benchmark]
    public bool TryDeserializeInvalidJson_SadPath()
    {
        try
        {
            JsonSerializer.Deserialize<TestModel>(_invalidJson, _options);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    [Benchmark]
    public bool TryDeserializeMalformedJson_SadPath()
    {
        try
        {
            JsonSerializer.Deserialize<TestModel>("{invalid json", _options);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    [Benchmark]
    public bool TrySerializeWithoutCloakIdResolver_SadPath()
    {
        try
        {
            // This should serialize without encoding (no CloakId resolver)
            var standardOptions = new JsonSerializerOptions();
            JsonSerializer.Serialize(_testModel, standardOptions);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    [Benchmark]
    public string SerializeLargeModel()
    {
        var largeModel = new LargeTestModel();
        return JsonSerializer.Serialize(largeModel, _options);
    }

    public class LargeTestModel
    {
        [CloakId] public int Id1 { get; set; } = 1;
        [CloakId] public int Id2 { get; set; } = 2;
        [CloakId] public int Id3 { get; set; } = 3;
        [CloakId] public int Id4 { get; set; } = 4;
        [CloakId] public int Id5 { get; set; } = 5;
        [CloakId] public long LongId1 { get; set; } = 1000000L;
        [CloakId] public long LongId2 { get; set; } = 2000000L;
        [CloakId] public uint UintId1 { get; set; } = 100u;
        [CloakId] public uint UintId2 { get; set; } = 200u;
        [CloakId] public short ShortId1 { get; set; } = 10;
        [CloakId] public short ShortId2 { get; set; } = 20;
        [CloakId] public ushort UshortId1 { get; set; } = 30;
        [CloakId] public ushort UshortId2 { get; set; } = 40;
        
        public string Name { get; set; } = "Large Model Test";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
