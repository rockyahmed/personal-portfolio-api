using UserManagement.Application.Common;
using UserManagement.Application.DTOs.User;

namespace UserManagement.Application.Interfaces;

/// <summary>
/// Business service contract for user lifecycle, profile, role, and status management.
/// </summary>
public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Guid id, UpdateUserRoleRequest request, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}
