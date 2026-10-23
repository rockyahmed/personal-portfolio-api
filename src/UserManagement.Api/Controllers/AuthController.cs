using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces;

namespace UserManagement.Api.Controllers;

/// <summary>
/// Authentication and session management endpoints.
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">New user registration credentials and profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Registration successful, returns JWT authentication tokens.</response>
    /// <response code="400">Invalid registration data or model validation failure.</response>
    /// <response code="409">Email or username is already taken.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return OkResponse(result, "Registration successful.");
    }

    /// <summary>
    /// Authenticate a user with email/username and password.
    /// </summary>
    /// <param name="request">Login credentials containing identifier and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Login successful, returns JWT access and refresh tokens.</response>
    /// <response code="400">Invalid credentials or account suspended.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return OkResponse(result, "Login successful.");
    }

    /// <summary>
    /// Refresh an expired or expiring access token using a valid refresh token.
    /// </summary>
    /// <param name="request">Current access token and refresh token pair.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">New access and refresh token pair issued.</response>
    /// <response code="400">Invalid or expired tokens.</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return OkResponse(result, "Tokens refreshed successfully.");
    }

    /// <summary>
    /// Revoke the active refresh token for the authenticated user (Log out).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Token revoked successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken(CancellationToken cancellationToken)
    {
        await _authService.RevokeTokenAsync(CurrentUserId, cancellationToken);
        return OkMessage("Active session revoked successfully.");
    }

    /// <summary>
    /// Retrieve the profile of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns current user profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User was not found.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUserAsync(CurrentUserId, cancellationToken);
        return OkResponse(user, "Current user retrieved successfully.");
    }
}
