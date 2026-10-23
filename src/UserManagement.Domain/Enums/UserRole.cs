namespace UserManagement.Domain.Enums;

/// <summary>
/// Defines user roles and authorization access tiers.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Standard user with regular permissions.
    /// </summary>
    User = 1,

    /// <summary>
    /// Manager with operational and elevated read/update permissions.
    /// </summary>
    Manager = 2,

    /// <summary>
    /// Administrator with full control over users and system operations.
    /// </summary>
    Admin = 3
}
