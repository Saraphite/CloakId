using CloakId;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Simulated in-memory data store
    private static readonly List<UserDto> _users =
    [
        new UserDto { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.UtcNow.AddDays(-30) },
        new UserDto { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.UtcNow.AddDays(-15) },
        new UserDto { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", CreatedAt = DateTime.UtcNow.AddDays(-7) }
    ];

    /// <summary>
    /// GET /api/users - Get all users
    /// </summary>
    /// <returns>List of all users with encoded IDs (via JSON serialization)</returns>
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(_users);
    }

    /// <summary>
    /// GET /api/users/{id} - Get a specific user by encoded ID
    /// The [Cloak] attribute automatically decodes the string ID to an integer
    /// </summary>
    /// <param name="id">The encoded user ID (e.g., "aB3" instead of "1")</param>
    /// <returns>User details if found</returns>
    [HttpGet("{id}")]
    public IActionResult GetUser([Cloak] int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// POST /api/users - Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user with encoded ID (via JSON serialization)</returns>
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { Message = "Name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { Message = "Email is required" });
        }

        // Check if email already exists
        if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(new { Message = "A user with this email already exists" });
        }

        // Create new user
        var newUser = new UserDto
        {
            Id = _users.Max(u => u.Id) + 1, // Simple ID generation
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _users.Add(newUser);

        return Created("/api/users/" + newUser.Id, newUser);
    }

    /// <summary>
    /// PUT /api/users/{id} - Update an existing user
    /// </summary>
    /// <param name="id">The encoded user ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id}")]
    public IActionResult UpdateUser([Cloak] int id, [FromBody] UpdateUserRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { Message = "Name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { Message = "Email is required" });
        }

        // Check if email already exists for another user
        if (_users.Any(u => u.Id != id && u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(new { Message = "A user with this email already exists" });
        }

        // Update user
        user.Name = request.Name;
        user.Email = request.Email;

        return Ok(user);
    }

    /// <summary>
    /// DELETE /api/users/{id} - Delete a user
    /// </summary>
    /// <param name="id">The encoded user ID</param>
    /// <returns>Confirmation of deletion</returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteUser([Cloak] int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        _users.Remove(user);

        return Ok(user);
    }
}

// DTOs and Request Models
public class UserDto
{
    [Cloak] // This will automatically encode the ID in JSON responses
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public record CreateUserRequest(string Name, string Email);

public record UpdateUserRequest(string Name, string Email);
