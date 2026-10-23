using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Security;

/// <summary>
/// Password hashing using BCrypt with enhanced work factor and SHA384 pre-hashing.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
