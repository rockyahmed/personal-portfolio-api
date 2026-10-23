using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Persistence;

/// <summary>
/// Database initial seed helper to provision initial administrator, demo users, and showcase projects.
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

            if (!await context.Projects.AnyAsync())
            {
                logger.LogInformation("No projects found. Seeding initial portfolio showcase projects...");

                var project1 = new PortfolioProject(
                    title: "Modern E-Commerce Platform",
                    description: "Full-stack scalable e-commerce platform with microservices architecture, secure JWT authentication, Stripe payment processing, and dynamic inventory management.",
                    imageUrl: "https://images.unsplash.com/photo-1557821552-17105176677c?w=800&auto=format&fit=crop&q=80",
                    link: "https://shop.example.com",
                    tags: "ASP.NET Core, React, Redux, PostgreSQL, Stripe, Docker",
                    githubUrl: "https://github.com/example/modern-ecommerce",
                    isFeatured: true,
                    displayOrder: 1);

                var project2 = new PortfolioProject(
                    title: "AI-Powered Productivity Workspace",
                    description: "Intelligent workspace utilizing large language models to automate task prioritization, document summarization, and team sprint scheduling.",
                    imageUrl: "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=800&auto=format&fit=crop&q=80",
                    link: "https://ai-workspace.example.com",
                    tags: ".NET 10, OpenAI, Next.js, TypeScript, TailwindCSS",
                    githubUrl: "https://github.com/example/ai-productivity-workspace",
                    isFeatured: true,
                    displayOrder: 2);

                var project3 = new PortfolioProject(
                    title: "Cloud Infrastructure Observability Dashboard",
                    description: "High-performance real-time telemetry dashboard monitoring Kubernetes cluster health, latency metrics, and API gateway throughput with live websockets.",
                    imageUrl: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&auto=format&fit=crop&q=80",
                    link: "https://telemetry.example.com",
                    tags: "ASP.NET Core, SignalR, Vue.js, Prometheus, Docker",
                    githubUrl: "https://github.com/example/cloud-observability",
                    isFeatured: false,
                    displayOrder: 3);

                await context.Projects.AddRangeAsync(project1, project2, project3);
                await context.SaveChangesAsync();

                logger.LogInformation("Default portfolio showcase projects seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
