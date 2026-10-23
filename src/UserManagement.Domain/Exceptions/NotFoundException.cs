namespace UserManagement.Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"Resource '{resourceName}' with key '{key}' was not found.", 404)
    {
    }
}
