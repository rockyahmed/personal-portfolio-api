namespace UserManagement.Domain.Entities;

/// <summary>
/// Domain entity representing a personal portfolio showcase project.
/// </summary>
public class PortfolioProject
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public string Link { get; private set; } = string.Empty;
    public string? GithubUrl { get; private set; }
    public string Tags { get; private set; } = string.Empty;
    public bool IsFeatured { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }

    // Required by EF Core
    protected PortfolioProject() { }

    public PortfolioProject(
        string title,
        string description,
        string imageUrl,
        string link,
        string? tags = null,
        string? githubUrl = null,
        bool isFeatured = false,
        int displayOrder = 0)
    {
        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        ImageUrl = imageUrl.Trim();
        Link = link.Trim();
        Tags = CleanTags(tags);
        GithubUrl = githubUrl?.Trim();
        IsFeatured = isFeatured;
        DisplayOrder = displayOrder;
        CreatedAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(
        string title,
        string description,
        string imageUrl,
        string link,
        string? tags,
        string? githubUrl,
        bool isFeatured,
        int displayOrder)
    {
        Title = title.Trim();
        Description = description.Trim();
        ImageUrl = imageUrl.Trim();
        Link = link.Trim();
        Tags = CleanTags(tags);
        GithubUrl = githubUrl?.Trim();
        IsFeatured = isFeatured;
        DisplayOrder = displayOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string CleanTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags)) return string.Empty;
        var split = tags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Join(",", split);
    }
}
