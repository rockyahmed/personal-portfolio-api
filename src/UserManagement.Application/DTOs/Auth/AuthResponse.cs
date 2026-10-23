using UserManagement.Application.DTOs.User;

namespace UserManagement.Application.DTOs.Auth;

/// <summary>
/// Successful authentication token payload with user profile.
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresInMinutes { get; set; }
    public DateTime ExpirationUtc { get; set; }
    public UserDto User { get; set; } = default!;
}
