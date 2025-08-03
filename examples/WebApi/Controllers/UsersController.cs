using CloakId;
using CloakId.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ICloakIdCodec codec) : ControllerBase
{

    /// <summary>
    /// Example 1: Manual conversion approach
    /// Route: GET api/users/manual/rs (where "rs" is an encoded ID for user 33)
    /// </summary>
    [HttpGet("manual/{encodedId}")]
    public IActionResult GetUserManual(string encodedId)
    {
        try
        {
            var userId = (int)codec.Decode(encodedId, typeof(int));
            return Ok(new { 
                Method = "Manual conversion",
                UserId = userId, 
                EncodedId = encodedId,
                Message = $"Successfully decoded '{encodedId}' to user ID {userId}"
            });
        }
        catch (ArgumentException)
        {
            return BadRequest($"Invalid user ID: {encodedId}");
        }
    }

    /// <summary>
    /// Example 2: Automatic conversion with model binder
    /// Route: GET api/users/auto/rs (where "rs" is an encoded ID for user 33)
    /// The [CloakId] attribute tells the model binder to automatically convert the string
    /// </summary>
    [HttpGet("auto/{id}")]
    public IActionResult GetUserAuto([CloakId] int id)
    {
        return Ok(new { 
            Method = "Automatic conversion (model binder)",
            UserId = id,
            EncodedId = codec.Encode(id, typeof(int)),
            Message = $"Parameter automatically converted to user ID {id}"
        });
    }

    /// <summary>
    /// Example 3: Multiple encoded parameters
    /// Route: GET api/users/rs/posts/PQ (where "rs" = user 33, "PQ" = post 20)
    /// </summary>
    [HttpGet("{userId}/posts/{postId}")]
    public IActionResult GetUserPost([CloakId] int userId, [CloakId] long postId)
    {
        return Ok(new { 
            Method = "Multiple automatic conversions",
            UserId = userId,
            PostId = postId,
            EncodedUserId = codec.Encode(userId, typeof(int)),
            EncodedPostId = codec.Encode(postId, typeof(long)),
            Message = $"User {userId} viewing post {postId}"
        });
    }

    /// <summary>
    /// Example 4: Fallback - works with both encoded strings and regular numbers
    /// Route: GET api/users/fallback/rs or GET api/users/fallback/33
    /// Note: Numeric values work because the model binder falls back to default parsing if decoding fails
    /// </summary>
    [HttpGet("fallback/{id}")]
    public IActionResult GetUserFallback([CloakId] int id)
    {
        // This works because the model binder falls back to default binding
        // if the string can't be decoded as a CloakId
        return Ok(new { 
            Method = "Fallback (works with encoded or numeric)",
            UserId = id,
            EncodedId = codec.Encode(id, typeof(int)),
            Message = $"Retrieved user {id} (supports both encoded strings and regular numbers)"
        });
    }

    /// <summary>
    /// Example 5: Create a user and return both numeric and encoded IDs
    /// Route: POST api/users
    /// </summary>
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        // Simulate creating a user and getting a new ID
        var newUserId = Random.Shared.Next(1000, 9999);
        var encodedId = codec.Encode(newUserId, typeof(int));
        
        return CreatedAtAction(
            nameof(GetUserAuto), 
            new { id = encodedId }, 
            new { 
                UserId = newUserId,
                EncodedId = encodedId,
                request.Name,
                Message = $"User created with ID {newUserId} (encoded as {encodedId})"
            });
    }

    /// <summary>
    /// Example 6: Strict encoded-only endpoint (when AllowNumericFallback = false)
    /// Route: GET api/users/strict/rs (only accepts encoded strings like "rs")
    /// This endpoint will return 400 Bad Request if you try: GET api/users/strict/33
    /// Note: The behavior depends on the AllowNumericFallback configuration in Program.cs
    /// </summary>
    [HttpGet("strict/{id}")]
    public IActionResult GetUserStrict([CloakId] int id)
    {
        return Ok(new { 
            Method = "Strict encoded-only (respects AllowNumericFallback setting)",
            UserId = id,
            EncodedId = codec.Encode(id, typeof(int)),
            Message = $"Retrieved user {id} - this endpoint behavior depends on AllowNumericFallback setting"
        });
    }
}

public record CreateUserRequest(string Name);
