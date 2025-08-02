using CloakId;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Example usage showing the new attribute-based approach

// Set up dependency injection
var services = new ServiceCollection();
services.AddCloakIdWithSqids(options =>
{
    options.MinLength = 6; // Configure minimum length
    // You can also set a custom alphabet:
    // options.Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
});
services.AddCloakId(); // Add the type info resolver

var serviceProvider = services.BuildServiceProvider();

// Get the type info resolver
var typeInfoResolver = serviceProvider.GetRequiredService<CloakIdTypeInfoResolver>();

// Configure JSON options with the resolver
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    TypeInfoResolver = typeInfoResolver
};

// Example 1: Attribute-based encoding
Console.WriteLine("=== Attribute-Based CloakId ===");

var user = new UserDto
{
    UserId = 12345,
    AccountId = 98765432109876,
    RegularId = 999, // This will remain as a number
    Name = "John Doe",
    OptionalId = 42
};

// Serialize to JSON - only [CloakId] properties become encoded strings
var json = JsonSerializer.Serialize(user, jsonOptions);
Console.WriteLine("Serialized JSON:");
Console.WriteLine(json);
Console.WriteLine();

// Deserialize from JSON
var deserializedUser = JsonSerializer.Deserialize<UserDto>(json, jsonOptions);
Console.WriteLine("Deserialized object:");
Console.WriteLine($"User ID: {deserializedUser!.UserId}");
Console.WriteLine($"Account ID: {deserializedUser.AccountId}");
Console.WriteLine($"Regular ID: {deserializedUser.RegularId}"); // This remains a number
Console.WriteLine($"Name: {deserializedUser.Name}");
Console.WriteLine($"Optional ID: {deserializedUser.OptionalId}");
Console.WriteLine();

// Example 2: Direct codec usage
Console.WriteLine("=== Direct Codec Usage ===");
var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();

var originalValue = 54321;
var encoded = codec.Encode(originalValue, typeof(int));
var decoded = (int)codec.Decode(encoded, typeof(int));

Console.WriteLine($"Original: {originalValue}");
Console.WriteLine($"Encoded: {encoded}");
Console.WriteLine($"Decoded: {decoded}");
Console.WriteLine();

// Example 3: Different numeric types
Console.WriteLine("=== Different Numeric Types ===");

var multiType = new MultiTypeDto
{
    IntId = 123,
    LongId = 456789012345,
    UIntId = 789,
    ShortId = 321
};

var multiJson = JsonSerializer.Serialize(multiType, jsonOptions);
Console.WriteLine("Multi-type JSON:");
Console.WriteLine(multiJson);

var deserializedMulti = JsonSerializer.Deserialize<MultiTypeDto>(multiJson, jsonOptions);
Console.WriteLine("\nDeserialized multi-type:");
Console.WriteLine($"Int ID: {deserializedMulti!.IntId}");
Console.WriteLine($"Long ID: {deserializedMulti.LongId}");
Console.WriteLine($"UInt ID: {deserializedMulti.UIntId}");
Console.WriteLine($"Short ID: {deserializedMulti.ShortId}");

// Example DTO using the new attribute approach
public class UserDto
{
    [CloakId]
    public int UserId { get; set; }
    
    [CloakId]
    public long AccountId { get; set; }
    
    // Regular properties without the attribute remain unchanged
    public int RegularId { get; set; }
    
    public string Name { get; set; } = null!;
    
    [CloakId]
    public int? OptionalId { get; set; }
}

public class MultiTypeDto
{
    [CloakId] public int IntId { get; set; }
    [CloakId] public long LongId { get; set; }
    [CloakId] public uint UIntId { get; set; }
    [CloakId] public short ShortId { get; set; }
}
