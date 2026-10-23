using UserManagement.Application.Common;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Infrastructure.Services;

/// <summary>
/// Service managing user entities, business constraints, and lifecycle operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var pagedUsers = await _userRepository.GetPagedAsync(filter, cancellationToken);
        var dtos = pagedUsers.Items.Select(UserDto.FromEntity).ToList();

        return new PagedResult<UserDto>(dtos, pagedUsers.TotalCount, pagedUsers.PageNumber, pagedUsers.PageSize);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        return UserDto.FromEntity(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check uniqueness of username
        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken: cancellationToken))
        {
            throw new ConflictException($"Username '{request.Username}' is already taken.");
        }

        // Check uniqueness of email
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken: cancellationToken))
        {
            throw new ConflictException($"Email address '{request.Email}' is already registered.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            username: request.Username,
            email: request.Email,
            firstName: request.FirstName,
            lastName: request.LastName,
            passwordHash: passwordHash,
            role: request.Role,
            status: request.Status,
            phoneNumber: request.PhoneNumber);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return UserDto.FromEntity(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return UserDto.FromEntity(user);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        user.SetStatus(request.Status);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleAsync(Guid id, UpdateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        user.SetRole(request.Role);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException("The current password provided is incorrect.");
        }

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.SetPassword(newHash);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
