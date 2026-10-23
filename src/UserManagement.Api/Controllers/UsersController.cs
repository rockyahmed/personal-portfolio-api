using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

/// <summary>
/// User management, administration, and profile endpoints.
/// </summary>
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieve a paginated list of users with search, filtering, and sorting.
    /// </summary>
    /// <param name="filter">Pagination, role, status, sorting, and search term query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the paginated collection of users.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Admin or Manager role.</response>
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUsersAsync(filter, cancellationToken);
        return OkResponse(result, "Users retrieved successfully.");
    }

    /// <summary>
    /// Retrieve a specific user by unique identifier.
    /// </summary>
    /// <param name="id">User unique identifier (Guid).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns user profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Users can only view their own profile unless they are Admin/Manager.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id != CurrentUserId && CurrentUserRole != nameof(UserRole.Admin) && CurrentUserRole != nameof(UserRole.Manager))
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        return OkResponse(user, "User details retrieved successfully.");
    }

    /// <summary>
    /// Create a new user with custom role and status (Administrator only).
    /// </summary>
    /// <param name="request">New user parameters including assigned role and status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Invalid payload or validation failure.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Administrator role.</response>
    /// <response code="409">Email or username is already taken.</response>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateUserAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetUserById), new { id = user.Id }, user, "User created successfully.");
    }

    /// <summary>
    /// Update personal profile information (name and phone number).
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="request">Updated profile details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Profile updated successfully.</response>
    /// <response code="400">Invalid payload.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Users can only edit their own profile unless they are Admin.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (id != CurrentUserId && CurrentUserRole != nameof(UserRole.Admin))
        {
            return Forbid();
        }

        var updatedUser = await _userService.UpdateUserAsync(id, request, cancellationToken);
        return OkResponse(updatedUser, "User profile updated successfully.");
    }

    /// <summary>
    /// Update a user's account status (Administrator only).
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="request">New lifecycle status (Active, Inactive, Suspended, PendingVerification).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Status changed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Administrator role.</response>
    /// <response code="404">User not found.</response>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateStatusAsync(id, request, cancellationToken);
        return OkMessage("User status updated successfully.");
    }

    /// <summary>
    /// Update a user's role access tier (Administrator only).
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="request">Target role (Admin, Manager, User).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Role updated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Administrator role.</response>
    /// <response code="404">User not found.</response>
    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateRoleAsync(id, request, cancellationToken);
        return OkMessage("User role updated successfully.");
    }

    /// <summary>
    /// Change password for a user account.
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="request">Current and new password payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="400">Current password incorrect or validation error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Users can only change their own password unless they are Admin.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("{id:guid}/change-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (id != CurrentUserId && CurrentUserRole != nameof(UserRole.Admin))
        {
            return Forbid();
        }

        await _userService.ChangePasswordAsync(id, request, cancellationToken);
        return OkMessage("Password changed successfully.");
    }

    /// <summary>
    /// Soft delete a user account from active system use (Administrator only).
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">User account deactivated/deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Administrator role.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == CurrentUserId)
        {
            return BadRequest(ApiResponse.FailureResult("Administrators cannot delete their own active account."));
        }

        await _userService.DeleteUserAsync(id, cancellationToken);
        return OkMessage("User deleted successfully.");
    }
}
