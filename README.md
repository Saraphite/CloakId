# CloakId

A .NET library that provides automatic encoding/decoding of numeric properties to obfuscated strings during JSON serialization using attributes. This helps prevent exposing internal numeric IDs in APIs while maintaining clean, readable code.

## Features

- **Attribute-based**: Simply mark properties with `[CloakId]` to enable encoding
- **Automatic JSON conversion**: Properties are automatically encoded to strings during serialization and decoded back during deserialization
- **Pluggable encoding**: Support for different encoding strategies (Sqids provided out of the box)
- **Dependency injection**: Full integration with Microsoft.Extensions.DependencyInjection
- **Type safety**: Compile-time type checking with support for all numeric types
- **Nullable support**: Full support for nullable numeric types

## Quick Start

### 1. Install packages

```xml
<PackageReference Include="CloakId" Version="1.0.0" />
<PackageReference Include="CloakId.Sqids" Version="1.0.0" />
```

### 2. Configure services

```csharp
using CloakId;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCloakIdWithSqids(minLength: 6); // Configure Sqids encoding
services.AddCloakId(); // Add the type info resolver

var serviceProvider = services.BuildServiceProvider();
```

### 3. Use the attribute in your DTOs

```csharp
public class UserDto
{
    [CloakId]
    public int UserId { get; set; }
    
    [CloakId]
    public long AccountId { get; set; }
    
    // Regular properties without the attribute remain unchanged
    public int RegularId { get; set; }
    
    public string Name { get; set; }
    
    [CloakId]
    public int? OptionalId { get; set; }
}
```

### 4. Configure JSON serialization

```csharp
var typeInfoResolver = serviceProvider.GetRequiredService<CloakIdTypeInfoResolver>();

var jsonOptions = new JsonSerializerOptions
{
    TypeInfoResolver = typeInfoResolver
};
```

### 5. Serialize/Deserialize

```csharp
var user = new UserDto
{
    UserId = 12345,
    AccountId = 98765432109876,
    RegularId = 999, // This remains as a number
    Name = "John Doe",
    OptionalId = 42
};

// Serialize - only [CloakId] properties become encoded strings
var json = JsonSerializer.Serialize(user, jsonOptions);
// Result: {"UserId":"A6das1","AccountId":"xnF9HulfM","RegularId":999,"Name":"John Doe","OptionalId":"JgaEBg"}

// Deserialize - strings decode back to original values
var deserializedUser = JsonSerializer.Deserialize<UserDto>(json, jsonOptions);
// deserializedUser.UserId == 12345
// deserializedUser.AccountId == 98765432109876
// deserializedUser.RegularId == 999 (unchanged)
```

## Supported Types

The `[CloakId]` attribute can be applied to the following numeric property types:

- `int` and `int?`
- `uint` and `uint?`
- `long` and `long?`
- `ulong` and `ulong?`
- `short` and `short?`
- `ushort` and `ushort?`

## Direct Codec Usage

You can also use the codec directly for manual encoding/decoding:

```csharp
var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();

var originalValue = 12345;
var encoded = codec.Encode(originalValue, typeof(int)); // "A6das1"
var decoded = (int)codec.Decode(encoded, typeof(int)); // 12345
```

## Configuration Options

### Sqids Configuration

```csharp
services.AddCloakIdWithSqids(
    alphabet: "abcdefghijklmnopqrstuvwxyz0123456789", // Custom alphabet
    minLength: 8 // Minimum length of encoded strings
);
```

### Attribute Configuration (Future Enhancement)

```csharp
public class UserDto
{
    [CloakId(MinLength = 8)]
    public int UserId { get; set; }
    
    [CloakId(Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    public long AccountId { get; set; }
}
```

## Custom Codecs

You can implement your own encoding strategy by implementing `ICloakIdCodec`:

```csharp
public class MyCustomCodec : ICloakIdCodec
{
    public string Encode(object value, Type valueType) { /* ... */ }
    public object Decode(string encodedValue, Type targetType) { /* ... */ }
}

// Register your codec
services.AddSingleton<ICloakIdCodec, MyCustomCodec>();
```

## Example Output

When serialized to JSON, your attributed properties will look like this:

```json
{
  "UserId": "A6das1",
  "AccountId": "xnF9HulfM",
  "RegularId": 999,
  "Name": "John Doe",
  "OptionalId": "JgaEBg"
}
```

Instead of exposing the raw numeric values:

```json
{
  "UserId": 12345,
  "AccountId": 98765432109876,
  "RegularId": 999,
  "Name": "John Doe", 
  "OptionalId": 42
}
```

Notice how only the properties marked with `[CloakId]` are encoded, while `RegularId` remains as a number.

## Performance

CloakId is designed for performance with minimal overhead. You can run comprehensive benchmarks to see the performance characteristics:

### Running Benchmarks

```bash
# Run all benchmarks
./run-benchmarks.ps1

# Run only encoding/decoding benchmarks
./run-benchmarks.ps1 "*Encode*"

# Run only JSON serialization benchmarks  
./run-benchmarks.ps1 "*Json*"

# Run only happy path tests
./run-benchmarks.ps1 "*HappyPath*"

# Run only error handling tests
./run-benchmarks.ps1 "*SadPath*"

# Quick validation run
./run-benchmarks.ps1 "*" --dry
```

### Sample Results

Based on benchmarks, typical performance characteristics:

- **Encoding**: ~4 microseconds per int32 value
- **JSON Serialization**: ~40 microseconds for small models
- **Memory allocation**: ~21KB allocated per serialization of typical models
- **Error handling**: Fast exception handling for invalid data

See `/benchmarks/README.md` for detailed benchmark information.

## Benefits

1. **Security**: Internal numeric IDs are not exposed in API responses
2. **Clean Code**: Simple attribute-based approach, no wrapper types needed
3. **Selective**: Choose exactly which properties to encode
4. **Type Safety**: Full support for nullable types and type checking
5. **Performance**: Efficient encoding/decoding with minimal overhead
6. **Flexibility**: Easy to swap encoding strategies without changing business logic

## License

This project is licensed under the MIT License - see the LICENSE file for details.