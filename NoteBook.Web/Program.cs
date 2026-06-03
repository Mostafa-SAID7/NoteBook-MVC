using NoteBook.Web.Configuration;
using Serilog;

// Initialize logging
LoggingConfiguration.Configure();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure application (all layers)
    ApplicationBootstrap.ConfigureApplication(builder);

    // Build application
    var app = builder.Build();

    // Run application
    await ApplicationBootstrap.RunAsync(app);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
