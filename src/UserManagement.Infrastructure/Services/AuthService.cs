using System.Security.Claims;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Infrastructure.Services;

/// <summary>
/// Service managing authentication, registration, JWT sessions, and security verification.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken: cancellationToken))
        {
            throw new ConflictException($"Username '{request.Username}' is already taken.");
        }

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
            role: UserRole.User,
            status: UserStatus.Active, // Active for seamless immediate use
            phoneNumber: request.PhoneNumber);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiryDays = 7;
        var refreshTokenExpiryUtc = DateTime.UtcNow.AddDays(expiryDays);

        user.SetRefreshToken(refreshToken, refreshTokenExpiryUtc);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresInMinutes = _tokenService.GetTokenExpiryMinutes(),
            ExpirationUtc = DateTime.UtcNow.AddMinutes(_tokenService.GetTokenExpiryMinutes()),
            User = UserDto.FromEntity(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email/username or password.");
        }

        if (user.Status == UserStatus.Suspended)
        {
            throw new ValidationException("This account has been suspended. Please contact support.");
        }

        if (user.Status == UserStatus.Inactive)
        {
            throw new ValidationException("This account is inactive. Please reactivate your account.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiryUtc = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken(refreshToken, refreshTokenExpiryUtc);
        user.RecordLogin();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresInMinutes = _tokenService.GetTokenExpiryMinutes(),
            ExpirationUtc = DateTime.UtcNow.AddMinutes(_tokenService.GetTokenExpiryMinutes()),
            User = UserDto.FromEntity(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            throw new ValidationException("Invalid access token or signature.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new ValidationException("Invalid user claims in access token.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTimeUtc <= DateTime.UtcNow)
        {
            throw new ValidationException("Invalid or expired refresh token. Please log in again.");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresInMinutes = _tokenService.GetTokenExpiryMinutes(),
            ExpirationUtc = DateTime.UtcNow.AddMinutes(_tokenService.GetTokenExpiryMinutes()),
            User = UserDto.FromEntity(user)
        };
    }

    public async Task RevokeTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        user.RevokeRefreshToken();
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        return UserDto.FromEntity(user);
    }
}
