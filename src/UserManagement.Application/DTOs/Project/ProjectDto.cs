using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs.Project;

/// <summary>
/// Data transfer object representing a portfolio showcase item.
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string? GithubUrl { get; set; }
    public string Tags { get; set; } = string.Empty;
    public IReadOnlyList<string> TagList => string.IsNullOrWhiteSpace(Tags)
        ? Array.Empty<string>()
        : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static ProjectDto FromEntity(PortfolioProject project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl,
            Link = project.Link,
            GithubUrl = project.GithubUrl,
            Tags = project.Tags,
            IsFeatured = project.IsFeatured,
            DisplayOrder = project.DisplayOrder,
            CreatedAtUtc = project.CreatedAtUtc,
            UpdatedAtUtc = project.UpdatedAtUtc
        };
    }
}
