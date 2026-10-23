using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Project;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

/// <summary>
/// Portfolio project showcase management endpoints.
/// </summary>
public class ProjectsController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieve all portfolio projects with optional tag filtering, search, and pagination.
    /// </summary>
    /// <param name="filter">Search term, tag, featured flag, sorting, and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns paginated portfolio projects.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectFilter filter, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectsAsync(filter, cancellationToken);
        return OkResponse(result, "Portfolio projects retrieved successfully.");
    }

    /// <summary>
    /// Retrieve only highlighted/featured portfolio projects for home showcase.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns featured projects list.</response>
    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeaturedProjects(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetFeaturedProjectsAsync(cancellationToken);
        return OkResponse(result, "Featured projects retrieved successfully.");
    }

    /// <summary>
    /// Retrieve a single portfolio project by ID.
    /// </summary>
    /// <param name="id">Project unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns project details.</response>
    /// <response code="404">Project not found.</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        return OkResponse(project, "Project details retrieved successfully.");
    }

    /// <summary>
    /// Create a new portfolio showcase project (Admin or Manager only).
    /// </summary>
    /// <param name="request">Project data with title, description, image, link, and tags.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">Project successfully created.</response>
    /// <response code="400">Invalid payload or validation failure.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Admin or Manager role.</response>
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var created = await _projectService.CreateProjectAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetProjectById), new { id = created.Id }, created, "Project created successfully.");
    }

    /// <summary>
    /// Update an existing portfolio project (Admin or Manager only).
    /// </summary>
    /// <param name="id">Project unique identifier.</param>
    /// <param name="request">Updated project properties.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Project updated successfully.</response>
    /// <response code="400">Invalid payload.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Admin or Manager role.</response>
    /// <response code="404">Project not found.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var updated = await _projectService.UpdateProjectAsync(id, request, cancellationToken);
        return OkResponse(updated, "Project updated successfully.");
    }

    /// <summary>
    /// Delete a portfolio project (Admin only).
    /// </summary>
    /// <param name="id">Project unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Project deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Requires Admin role.</response>
    /// <response code="404">Project not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _projectService.DeleteProjectAsync(id, cancellationToken);
        return OkMessage("Project deleted successfully.");
    }
}
