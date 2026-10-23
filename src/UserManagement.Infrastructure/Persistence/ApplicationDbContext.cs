using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for User Management and Portfolio Showcase.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PortfolioProject> Projects => Set<PortfolioProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(u => u.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(u => u.RefreshToken)
                .HasMaxLength(256);

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasQueryFilter(u => !u.IsDeleted);
        });

        // PortfolioProject Entity Configuration
        modelBuilder.Entity<PortfolioProject>(entity =>
        {
            entity.ToTable("Projects");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(p => p.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(p => p.Link)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(p => p.GithubUrl)
                .HasMaxLength(500);

            entity.Property(p => p.Tags)
                .HasMaxLength(300);

            entity.HasIndex(p => p.IsFeatured);
            entity.HasIndex(p => p.DisplayOrder);

            entity.HasQueryFilter(p => !p.IsDeleted);
        });
    }
}
