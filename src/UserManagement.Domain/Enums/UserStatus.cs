namespace UserManagement.Domain.Enums;

/// <summary>
/// Represents the account activation and verification lifecycle state.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is active and allowed full access.
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is inactive or disabled.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// User account is suspended due to policy or administrative action.
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// User account is awaiting email or administrator verification.
    /// </summary>
    PendingVerification = 4
}
