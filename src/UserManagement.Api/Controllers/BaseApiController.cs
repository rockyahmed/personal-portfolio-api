using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;

namespace UserManagement.Api.Controllers;

/// <summary>
/// Abstract base API controller providing shared utility methods and claim helpers.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Gets the unique identifier of the authenticated user from token claims.
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? User.FindFirstValue("sub");

            return Guid.TryParse(claimValue, out var id) ? id : Guid.Empty;
        }
    }

    /// <summary>
    /// Gets the role claim string of the authenticated user.
    /// </summary>
    protected string? CurrentUserRole => User.FindFirstValue(ClaimTypes.Role);

    /// <summary>
    /// Returns a 200 OK wrapped ApiResponse.
    /// </summary>
    protected IActionResult OkResponse<T>(T data, string message = "Request successful.")
    {
        return Ok(ApiResponse<T>.SuccessResult(data, message, StatusCodes.Status200OK));
    }

    /// <summary>
    /// Returns a 201 Created wrapped ApiResponse.
    /// </summary>
    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data, string message = "Resource created successfully.")
    {
        var response = ApiResponse<T>.SuccessResult(data, message, StatusCodes.Status201Created);
        return CreatedAtAction(actionName, routeValues, response);
    }

    /// <summary>
    /// Returns a 200 OK message response without data payload.
    /// </summary>
    protected IActionResult OkMessage(string message = "Operation completed successfully.")
    {
        return Ok(ApiResponse.SuccessResult(message, StatusCodes.Status200OK));
    }
}
