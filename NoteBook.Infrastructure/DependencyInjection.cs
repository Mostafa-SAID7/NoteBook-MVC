namespace NoteBook.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using NoteBook.Domain.Repositories;
using NoteBook.Infrastructure.Data;
using NoteBook.Infrastructure.Repositories;

/// <summary>
/// Extension methods for registering Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        string connectionString)
    {
        // Database connection factory
        services.AddSingleton<IDbConnectionFactory>(
            new DbConnectionFactory(connectionString));
        
        // Repositories
        services.AddScoped<INoteRepository, NoteRepository>();
        
        return services;
    }
}
