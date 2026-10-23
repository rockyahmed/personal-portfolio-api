using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Project;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

/// <summary>
/// Data persistence contract for Portfolio Project items.
/// </summary>
public interface IProjectRepository
{
    Task<PortfolioProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<PortfolioProject>> GetPagedAsync(ProjectFilter filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PortfolioProject>> GetFeaturedAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PortfolioProject project, CancellationToken cancellationToken = default);
    Task UpdateAsync(PortfolioProject project, CancellationToken cancellationToken = default);
    Task DeleteAsync(PortfolioProject project, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
