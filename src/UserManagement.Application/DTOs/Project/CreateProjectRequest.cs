using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.Project;

/// <summary>
/// Payload to create a new portfolio showcase project.
/// </summary>
public class CreateProjectRequest
{
    [Required(ErrorMessage = "Project title is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Project description is required.")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cover image URL is required.")]
    [Url(ErrorMessage = "Please provide a valid image URL.")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Project live demo or website link is required.")]
    [Url(ErrorMessage = "Please provide a valid project URL.")]
    public string Link { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated tags (e.g. "React, .NET Core, Docker, PostgreSQL")
    /// </summary>
    [StringLength(300, ErrorMessage = "Tags cannot exceed 300 characters.")]
    public string? Tags { get; set; }

    [Url(ErrorMessage = "Please provide a valid GitHub URL.")]
    public string? GithubUrl { get; set; }

    public bool IsFeatured { get; set; } = false;

    public int DisplayOrder { get; set; } = 0;
}
