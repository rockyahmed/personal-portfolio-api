namespace UserManagement.Application.DTOs.Project;

/// <summary>
/// Query parameters for filtering, searching, and paginating portfolio projects.
/// </summary>
public class ProjectFilter
{
    private const int MaxPageSize = 100;
    private int _pageSize = 12;
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
    /// Number of projects per page. Defaults to 12.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
    }

    /// <summary>
    /// Free-text search matching title, description, or tags.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter projects by specific technology tag (e.g. "React", ".NET", "Flutter").
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Filter only featured projects.
    /// </summary>
    public bool? IsFeatured { get; set; }

    /// <summary>
    /// Sort attribute ("displayorder", "createdat", "title"). Defaults to "displayorder".
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Set to true for descending order.
    /// </summary>
    public bool SortDescending { get; set; } = false;
}
