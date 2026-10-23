using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.Auth;

/// <summary>
/// Payload to refresh an expired or expiring access token.
/// </summary>
public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Access token is required.")]
    public string AccessToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
