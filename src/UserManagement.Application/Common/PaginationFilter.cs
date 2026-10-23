using UserManagement.Domain.Enums;

namespace UserManagement.Application.Common;

/// <summary>
/// Query parameters for pagination, searching, filtering, and sorting.
/// </summary>
public class PaginationFilter
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    /// <summary>
    /// Current 1-based page index. Defaults to 1.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Number of items per page. Defaults to 10, maximum is 100.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
    }

    /// <summary>
    /// Free-text search term applied against username, email, firstName, and lastName.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter users by specific role.
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Filter users by status.
    /// </summary>
    public UserStatus? Status { get; set; }

    /// <summary>
    /// Property name to sort by (e.g. "CreatedAt", "Email", "Username", "LastName").
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Set to true for descending order, false for ascending. Defaults to false.
    /// </summary>
    public bool SortDescending { get; set; } = false;
}
