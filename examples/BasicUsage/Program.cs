using CloakId;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Example usage showing how to use CloakId library

// Set up dependency injection
var services = new ServiceCollection();
services.AddCloakIdWithSqids(minLength: 6); // Minimum length of 6 characters
services.AddCloakId(); // Add the JSON converter factory

var serviceProvider = services.BuildServiceProvider();

// Get the codec
var codec = serviceProvider.GetRequiredService<ICloakIdCodec>();

// Get the JSON converter factory
var converterFactory = serviceProvider.GetRequiredService<IdJsonConverterFactory>();

// Configure JSON options
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true
};
jsonOptions.Converters.Add(converterFactory);

// Example 1: Direct encoding/decoding
Console.WriteLine("=== Direct Codec Usage ===");
var userId = new Id<int>(12345);
var encoded = codec.Encode(userId);
Console.WriteLine($"Original ID: {userId.Value}");
Console.WriteLine($"Encoded ID: {encoded}");

var decoded = codec.Decode<Id<int>>(encoded);
Console.WriteLine($"Decoded ID: {decoded.Value}");
Console.WriteLine();

// Example 2: JSON Serialization
Console.WriteLine("=== JSON Serialization ===");

var user = new UserDto
{
    UserId = new Id<int>(98765),
    AccountId = new Id<long>(123456789012345),
    Name = "John Doe"
};

// Serialize to JSON
var json = JsonSerializer.Serialize(user, jsonOptions);
Console.WriteLine("Serialized JSON:");
Console.WriteLine(json);
Console.WriteLine();

// Deserialize from JSON
var deserializedUser = JsonSerializer.Deserialize<UserDto>(json, jsonOptions);
Console.WriteLine("Deserialized object:");
Console.WriteLine($"User ID: {deserializedUser!.UserId.Value}");
Console.WriteLine($"Account ID: {deserializedUser.AccountId.Value}");
Console.WriteLine($"Name: {deserializedUser.Name}");
Console.WriteLine();

// Example 3: Using the convenience Id class (inherits from Id<int>)
Console.WriteLine("=== Convenience Id Class ===");
var simpleId = new Id(42);
var simpleEncoded = codec.Encode(simpleId);
Console.WriteLine($"Simple ID: {simpleId.Value}");
Console.WriteLine($"Encoded: {simpleEncoded}");

var simpleDecoded = codec.Decode<Id>(simpleEncoded);
Console.WriteLine($"Decoded: {simpleDecoded.Value}");

// Define a sample DTO with different ID types
public class UserDto
{
    public Id<int> UserId { get; set; } = null!;
    public Id<long> AccountId { get; set; } = null!;
    public string Name { get; set; } = null!;
}
