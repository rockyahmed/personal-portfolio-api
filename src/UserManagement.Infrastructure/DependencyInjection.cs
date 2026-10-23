using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Services;

namespace UserManagement.Infrastructure;

/// <summary>
/// Dependency injection registrations for Infrastructure components.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=usermanagement.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // Options
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Core Infrastructure Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        // Portfolio Projects Services
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}
