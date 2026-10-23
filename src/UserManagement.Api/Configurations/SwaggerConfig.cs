using System.Reflection;
using Microsoft.OpenApi;

namespace UserManagement.Api.Configurations;

/// <summary>
/// Swagger and OpenAPI specification configuration.
/// </summary>
public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "User Management API",
                Version = "v1",
                Description = "A production-grade, Clean Architecture REST API for User Lifecycle Management with JWT Bearer Authentication, Role-based Access Control (RBAC), EF Core with SQLite, and comprehensive Swagger documentation.",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "support@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Configure JWT Bearer Authentication in Swagger UI
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token format: `Bearer {your token}`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            var schemeReference = new OpenApiSecuritySchemeReference("Bearer");

            options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    schemeReference,
                    new List<string>()
                }
            });

            // Include XML comments from API assembly if present
            var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
            if (File.Exists(apiXmlPath))
            {
                options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
            }

            // Include XML comments from Application assembly if present
            var appXmlPath = Path.Combine(AppContext.BaseDirectory, "UserManagement.Application.xml");
            if (File.Exists(appXmlPath))
            {
                options.IncludeXmlComments(appXmlPath);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
            c.RoutePrefix = "swagger"; // Available at /swagger
            c.DisplayRequestDuration();
            c.EnablePersistAuthorization();
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });

        return app;
    }
}
