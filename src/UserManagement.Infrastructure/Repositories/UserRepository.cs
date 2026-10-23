using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core repository implementation for user persistence operations.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername, cancellationToken);
    }

    public async Task<User?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var normalized = identifier.Trim().ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == normalized || u.Email == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail && (!excludeUserId.HasValue || u.Id != excludeUserId.Value), cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Username == normalizedUsername && (!excludeUserId.HasValue || u.Id != excludeUserId.Value), cancellationToken);
    }

    public async Task<PagedResult<User>> GetPagedAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _context.Users.AsNoTracking();

        // Search term filter
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var search = filter.SearchTerm.Trim().ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        // Role filter
        if (filter.Role.HasValue)
        {
            query = query.Where(u => u.Role == filter.Role.Value);
        }

        // Status filter
        if (filter.Status.HasValue)
        {
            query = query.Where(u => u.Status == filter.Status.Value);
        }

        // Total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = (filter.SortBy?.ToLowerInvariant(), filter.SortDescending) switch
        {
            ("email", false) => query.OrderBy(u => u.Email),
            ("email", true) => query.OrderByDescending(u => u.Email),
            ("username", false) => query.OrderBy(u => u.Username),
            ("username", true) => query.OrderByDescending(u => u.Username),
            ("firstname", false) => query.OrderBy(u => u.FirstName),
            ("firstname", true) => query.OrderByDescending(u => u.FirstName),
            ("lastname", false) => query.OrderBy(u => u.LastName),
            ("lastname", true) => query.OrderByDescending(u => u.LastName),
            ("role", false) => query.OrderBy(u => u.Role),
            ("role", true) => query.OrderByDescending(u => u.Role),
            ("status", false) => query.OrderBy(u => u.Status),
            ("status", true) => query.OrderByDescending(u => u.Status),
            ("createdat", false) => query.OrderBy(u => u.CreatedAtUtc),
            ("createdat", true) => query.OrderByDescending(u => u.CreatedAtUtc),
            _ => query.OrderByDescending(u => u.CreatedAtUtc)
        };

        // Paging
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        // Executes soft delete on aggregate
        user.SoftDelete();
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
