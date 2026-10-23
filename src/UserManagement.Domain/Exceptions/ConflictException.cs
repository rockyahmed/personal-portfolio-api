namespace UserManagement.Domain.Exceptions;

/// <summary>
/// Thrown when an entity conflict occurs (e.g. duplicate username or email).
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409)
    {
    }
}
