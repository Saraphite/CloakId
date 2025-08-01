# CloakId Benchmarks

This project contains performance benchmarks for the CloakId library using BenchmarkDotNet.

## Running the Benchmarks

To run all benchmarks:

```bash
dotnet run --project benchmarks/CloakId.Benchmarks/CloakId.Benchmarks.csproj --configuration Release
```

## Benchmark Categories

### SqidsCodecBenchmarks
Tests the performance of the core encoding/decoding operations:

**Happy Path Tests:**
- `EncodeInt32_HappyPath` - Encoding a standard int value
- `EncodeUInt32_HappyPath` - Encoding a uint value
- `EncodeLong_HappyPath` - Encoding a long value
- `EncodeNullableInt_HappyPath` - Encoding a nullable int
- `DecodeInt32_HappyPath` - Decoding a valid encoded string
- `DecodeNullableInt_HappyPath` - Decoding to a nullable int
- `EncodeDecodeRoundTrip_Int32` - Full encode/decode cycle

**Sad Path Tests:**
- `TryEncodeUnsupportedType_SadPath` - Attempting to encode unsupported types
- `TryDecodeInvalidValue_SadPath` - Attempting to decode invalid encoded values
- `TryDecodeEmptyString_SadPath` - Attempting to decode empty strings
- `TryDecodeNullString_SadPath` - Attempting to decode null strings

### JsonSerializationBenchmarks
Tests the performance of JSON serialization with CloakId attributes:

**Happy Path Tests:**
- `SerializeModel_HappyPath` - Serializing a model with CloakId attributes
- `DeserializeModel_HappyPath` - Deserializing a valid JSON with encoded IDs
- `SerializeDeserializeRoundTrip` - Full JSON round-trip
- `SerializeModelWithNulls` - Handling nullable properties
- `SerializeLargeModel` - Performance with many CloakId properties

**Sad Path Tests:**
- `TryDeserializeInvalidJson_SadPath` - Deserializing JSON with invalid encoded values
- `TryDeserializeMalformedJson_SadPath` - Handling malformed JSON
- `TrySerializeWithoutCloakIdResolver_SadPath` - Serialization without CloakId resolver

## Expected Results

The benchmarks will provide insights into:
- Encoding/decoding performance for different numeric types
- JSON serialization overhead introduced by CloakId
- Error handling performance in edge cases
- Memory allocation patterns

## Notes

- Benchmarks are configured with `[MemoryDiagnoser]` to track memory allocations
- All benchmarks use the default Sqids configuration
- Sad path tests measure exception handling performance
