namespace NoteBook.Web.Configuration;

using Serilog;

/// <summary>
/// Application bootstrap orchestrator
/// Coordinates the startup sequence of the application
/// </summary>
public static class ApplicationBootstrap
{
    /// <summary>
    /// Initialize and configure the entire application
    /// Coordinates all configuration steps in the correct order
    /// </summary>
    public static void ConfigureApplication(WebApplicationBuilder builder)
    {
        // 1. Setup logging first
        LoggingConfiguration.AddSerilog(builder);

        // 2. Add authentication
        AuthenticationConfiguration.AddJwtAuthentication(builder);

        // 3. Add CORS
        CorsConfiguration.AddCorsPolicy(builder);

        // 4. Add rate limiting
        RateLimitingConfiguration.AddRateLimiting(builder);

        // 5. Register all services
        ServiceRegistrationConfiguration.RegisterServices(builder);
    }

    /// <summary>
    /// Run the application with error handling
    /// </summary>
    public static async Task RunAsync(WebApplication app)
    {
        try
        {
            // Configure the middleware pipeline
            PipelineConfiguration.ConfigureMiddleware(app);

            Log.Information("NoteBook application starting...");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
