using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities;

/// <summary>
/// Core aggregate entity representing a system user.
/// </summary>
public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.User;
    public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }
    
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTimeUtc { get; private set; }
    
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    // Required for EF Core
    protected User() { }

    public User(
        string username,
        string email,
        string firstName,
        string lastName,
        string passwordHash,
        UserRole role = UserRole.User,
        UserStatus status = UserStatus.Active,
        string? phoneNumber = null)
    {
        Id = Guid.NewGuid();
        Username = username.Trim().ToLowerInvariant();
        Email = email.Trim().ToLowerInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PasswordHash = passwordHash;
        Role = role;
        Status = status;
        PhoneNumber = phoneNumber?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetStatus(UserStatus newStatus)
    {
        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetRefreshToken(string token, DateTime expiryTimeUtc)
    {
        RefreshToken = token;
        RefreshTokenExpiryTimeUtc = expiryTimeUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTimeUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
