namespace UserManagement.Infrastructure.Security;

/// <summary>
/// Configuration settings for JSON Web Token (JWT) issuing and validation.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = "YourSuperSecretKeyForJwtTokenGeneration2026!";
    public string Issuer { get; set; } = "UserManagementApi";
    public string Audience { get; set; } = "UserManagementApiAudience";
    public int ExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
