namespace UserManagement.Application.Interfaces;

/// <summary>
/// Secure password hashing and verification contract.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
