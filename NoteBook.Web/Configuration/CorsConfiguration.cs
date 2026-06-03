namespace NoteBook.Web.Configuration;

/// <summary>
/// CORS (Cross-Origin Resource Sharing) configuration
/// Handles cross-origin request policies
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// Configure CORS policy for allowing all origins
    /// </summary>
    public static void AddCorsPolicy(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    /// <summary>
    /// Apply CORS middleware to the pipeline
    /// </summary>
    public static void UseCorsPolicy(WebApplication app)
    {
        app.UseCors("AllowAll");
    }
}
