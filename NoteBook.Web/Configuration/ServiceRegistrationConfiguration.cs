namespace NoteBook.Web.Configuration;

using NoteBook.Application;
using NoteBook.Infrastructure;
using Serilog;

/// <summary>
/// Service registration configuration
/// Handles dependency injection setup for all application services
/// </summary>
public static class ServiceRegistrationConfiguration
{
    /// <summary>
    /// Register all application services and layers
    /// </summary>
    public static void RegisterServices(WebApplicationBuilder builder)
    {
        // Get connection string
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");

        // Add core services
        builder.Services.AddControllersWithViews();
        
        // Add application and infrastructure layer services
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(connectionString);

        // Add Serilog
        builder.Host.UseSerilog();
    }
}
