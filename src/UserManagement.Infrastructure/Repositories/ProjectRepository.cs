using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs.Project;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core repository implementation for Portfolio projects.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PortfolioProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PagedResult<PortfolioProject>> GetPagedAsync(ProjectFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<PortfolioProject> query = _context.Projects.AsNoTracking();

        // Search term matching title, description or tags
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var search = filter.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search) ||
                p.Tags.ToLower().Contains(search));
        }

        // Tag filter
        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            var tag = filter.Tag.Trim().ToLower();
            query = query.Where(p => p.Tags.ToLower().Contains(tag));
        }

        // Featured filter
        if (filter.IsFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == filter.IsFeatured.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = (filter.SortBy?.ToLowerInvariant(), filter.SortDescending) switch
        {
            ("title", false) => query.OrderBy(p => p.Title),
            ("title", true) => query.OrderByDescending(p => p.Title),
            ("createdat", false) => query.OrderBy(p => p.CreatedAtUtc),
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAtUtc),
            ("displayorder", true) => query.OrderByDescending(p => p.DisplayOrder).ThenByDescending(p => p.CreatedAtUtc),
            _ => query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedAtUtc)
        };

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PortfolioProject>(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<IReadOnlyList<PortfolioProject>> GetFeaturedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.IsFeatured)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PortfolioProject project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
    }

    public Task UpdateAsync(PortfolioProject project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PortfolioProject project, CancellationToken cancellationToken = default)
    {
        project.SoftDelete();
        _context.Projects.Update(project);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
