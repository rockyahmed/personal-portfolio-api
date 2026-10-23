using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Persistence;

/// <summary>
/// Database initial seed helper to provision initial administrator and demo users.
/// </summary>
public static class UserDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        try
        {
            // Ensure SQLite database and tables are created
            await context.Database.EnsureCreatedAsync();

            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Database empty. Seeding initial users...");

                var admin = new User(
                    username: "admin",
                    email: "admin@example.com",
                    firstName: "System",
                    lastName: "Administrator",
                    passwordHash: passwordHasher.HashPassword("Admin@123456"),
                    role: UserRole.Admin,
                    status: UserStatus.Active,
                    phoneNumber: "+1234567890");

                var manager = new User(
                    username: "manager",
                    email: "manager@example.com",
                    firstName: "Operations",
                    lastName: "Manager",
                    passwordHash: passwordHasher.HashPassword("Manager@123456"),
                    role: UserRole.Manager,
                    status: UserStatus.Active,
                    phoneNumber: "+1987654321");

                var regularUser = new User(
                    username: "johndoe",
                    email: "john.doe@example.com",
                    firstName: "John",
                    lastName: "Doe",
                    passwordHash: passwordHasher.HashPassword("User@123456"),
                    role: UserRole.User,
                    status: UserStatus.Active,
                    phoneNumber: "+1555123456");

                await context.Users.AddRangeAsync(admin, manager, regularUser);
                await context.SaveChangesAsync();

                logger.LogInformation("Default seed users created successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding initial database users.");
            throw;
        }
    }
}
