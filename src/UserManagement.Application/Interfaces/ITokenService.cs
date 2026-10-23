using System.Security.Claims;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

/// <summary>
/// Service contract for generating and validating JSON Web Tokens (JWT) and refresh tokens.
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    int GetTokenExpiryMinutes();
}
