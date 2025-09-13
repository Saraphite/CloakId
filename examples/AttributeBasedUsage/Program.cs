using System.Text.Json;
using CloakId;
using CloakId.Abstractions;
using CloakId.Sqids;
using Microsoft.Extensions.DependencyInjection;

// Example usage showing the new attribute-based approach

Console.WriteLine("Testing CloakIdOptions Validation");
Console.WriteLine("==================================");

// Test validation examples
TestValidation();

Console.WriteLine("\nNow testing normal functionality with valid options...");
Console.WriteLine("=====================================================");

// Set up dependency injection with valid options
var services = new ServiceCollection();
services.AddCloakId().WithSqids(options =>
{
    options.MinLength = 6; // Configure minimum length
    // You can also set a custom alphabet:
    // options.Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
});

var serviceProvider = services.BuildServiceProvider();

static void TestValidation()
{
    Console.WriteLine("Testing MinLength validation:");
    try
    {
        var options1 = new CloakIdOptions { MinLength = 5 };
        WritePass($"Valid MinLength = {options1.MinLength}");
    }
    catch (Exception ex)
    {
        WriteFail($"Error: {ex.Message}");
    }

    try
    {
        var options2 = new CloakIdOptions { MinLength = -1 };
        WriteFail($"Invalid MinLength accepted: {options2.MinLength}");
    }
    catch (Exception ex)
    {
        WritePass($"Validation caught invalid MinLength: {ex.Message}");
    }

    Console.WriteLine("\nTesting Alphabet validation:");
    try
    {
        var options3 = new CloakIdOptions { Alphabet = "abcdefghijklmnopqrstuvwxyz" };
        WritePass("Valid alphabet accepted");
    }
    catch (Exception ex)
    {
        WriteFail($"Error with valid alphabet: {ex.Message}");
    }

    try
    {
        var options4 = new CloakIdOptions { Alphabet = "ab" };
        WriteFail("Short alphabet accepted");
    }
    catch (Exception ex)
    {
        WritePass($"Validation caught short alphabet: {ex.Message}");
    }

    try
    {
        var options5 = new CloakIdOptions { Alphabet = "aab" };
        WriteFail("Duplicate characters accepted");
    }
    catch (Exception ex)
    {
        WritePass($"Validation caught duplicates: {ex.Message}");
    }

    try
    {
        var options6 = new CloakIdOptions { Alphabet = "ab c" };
        WriteFail("Whitespace characters accepted");
    }
    catch (Exception ex)
    {
        WritePass($"Validation caught whitespace: {ex.Message}");
    }

    try
    {
        var options7 = new CloakIdOptions { Alphabet = "abc-def" };
        WriteFail("Special characters accepted");
    }
    catch (Exception ex)
    {
        WritePass($"Validation caught special characters: {ex.Message}");
    }
}

static void WritePass(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("  PASS ");
    Console.ResetColor();
    Console.WriteLine(message);
}

static void WriteFail(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("  FAIL ");
    Console.ResetColor();
    Console.WriteLine(message);
}

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
    [Cloak]
    public int UserId { get; set; }

    [Cloak]
    public long AccountId { get; set; }

    // Regular properties without the attribute remain unchanged
    public int RegularId { get; set; }

    public string Name { get; set; } = null!;

    [Cloak]
    public int? OptionalId { get; set; }
}

public class MultiTypeDto
{
    [Cloak] public int IntId { get; set; }
    [Cloak] public long LongId { get; set; }
    [Cloak] public uint UIntId { get; set; }
    [Cloak] public short ShortId { get; set; }
}
