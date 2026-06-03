namespace NoteBook.Web.Configuration;

using NoteBook.Web.Middleware;

/// <summary>
/// HTTP pipeline middleware configuration
/// Handles the order and setup of middleware components in the request pipeline
/// </summary>
public static class PipelineConfiguration
{
    /// <summary>
    /// Configure the HTTP request pipeline with all middleware
    /// </summary>
    public static void ConfigureMiddleware(WebApplication app)
    {
        // Add rate limiting first (before other middleware)
        RateLimitingConfiguration.UseRateLimiting(app);

        // Add global exception handling
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        // Configure Swagger UI (available in all environments)
        SwaggerConfiguration.UseSwaggerUI(app);

        // Configure environment-specific pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Add authentication and authorization
        app.UseAuthentication();
        CorsConfiguration.UseCorsPolicy(app);
        app.UseAuthorization();

        // Map static assets and controllers
        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        // Handle 404 - Not Found routes (catch-all at the end)
        app.MapFallback((context) =>
        {
            context.Request.RouteValues["controller"] = "Home";
            context.Request.RouteValues["action"] = "PageNotFound";
            return Task.CompletedTask;
        });
    }
}
