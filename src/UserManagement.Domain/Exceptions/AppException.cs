namespace UserManagement.Domain.Exceptions;

/// <summary>
/// Base domain exception for all domain-specific errors.
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}
