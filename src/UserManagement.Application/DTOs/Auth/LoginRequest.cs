using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.Auth;

/// <summary>
/// User login credentials request.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Username or Email address of the account.
    /// </summary>
    [Required(ErrorMessage = "Username or Email is required.")]
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Account password.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}
