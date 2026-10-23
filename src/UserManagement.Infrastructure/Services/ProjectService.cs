using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Project;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Infrastructure.Services;

/// <summary>
/// Business service implementation managing portfolio project showcases.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<PagedResult<ProjectDto>> GetProjectsAsync(ProjectFilter filter, CancellationToken cancellationToken = default)
    {
        var paged = await _projectRepository.GetPagedAsync(filter, cancellationToken);
        var dtos = paged.Items.Select(ProjectDto.FromEntity).ToList();

        return new PagedResult<ProjectDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetFeaturedProjectsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _projectRepository.GetFeaturedAsync(cancellationToken);
        return items.Select(ProjectDto.FromEntity).ToList();
    }

    public async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(PortfolioProject), id);
        }

        return ProjectDto.FromEntity(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new PortfolioProject(
            title: request.Title,
            description: request.Description,
            imageUrl: request.ImageUrl,
            link: request.Link,
            tags: request.Tags,
            githubUrl: request.GithubUrl,
            isFeatured: request.IsFeatured,
            displayOrder: request.DisplayOrder);

        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return ProjectDto.FromEntity(project);
    }

    public async Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(PortfolioProject), id);
        }

        project.Update(
            title: request.Title,
            description: request.Description,
            imageUrl: request.ImageUrl,
            link: request.Link,
            tags: request.Tags,
            githubUrl: request.GithubUrl,
            isFeatured: request.IsFeatured,
            displayOrder: request.DisplayOrder);

        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return ProjectDto.FromEntity(project);
    }

    public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(PortfolioProject), id);
        }

        await _projectRepository.DeleteAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);
    }
}
