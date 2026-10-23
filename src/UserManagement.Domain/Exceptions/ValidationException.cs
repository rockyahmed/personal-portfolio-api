namespace UserManagement.Domain.Exceptions;

/// <summary>
/// Thrown when input data fails validation requirements.
/// </summary>
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message, 400)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures occurred.", 400)
    {
        Errors = errors;
    }
}
