using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Project;

namespace UserManagement.Application.Interfaces;

/// <summary>
/// Service contract managing portfolio projects showcase logic.
/// </summary>
public interface IProjectService
{
    Task<PagedResult<ProjectDto>> GetProjectsAsync(ProjectFilter filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectDto>> GetFeaturedProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);
}
