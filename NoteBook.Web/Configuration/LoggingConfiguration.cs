namespace NoteBook.Web.Configuration;

using Serilog;

/// <summary>
/// Logging configuration for Serilog
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configure Serilog logging before application builder
    /// </summary>
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/app-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    /// <summary>
    /// Add Serilog to host
    /// </summary>
    public static void AddSerilog(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();
    }
}
