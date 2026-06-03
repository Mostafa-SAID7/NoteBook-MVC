namespace NoteBook.Application.DTOs;

/// <summary>
/// Health check response model
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Service status (Healthy, Degraded, Unhealthy)
    /// </summary>
    public string Status { get; set; } = "Healthy";
    
    /// <summary>
    /// Service name
    /// </summary>
    public string Service { get; set; } = "NoteBook API";
    
    /// <summary>
    /// Current timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// API version
    /// </summary>
    public string Version { get; set; } = "2.0.0";
    
    /// <summary>
    /// Environment (Development, Staging, Production)
    /// </summary>
    public string? Environment { get; set; }
    
    /// <summary>
    /// Database connectivity status
    /// </summary>
    public bool DatabaseConnected { get; set; }
    
    /// <summary>
    /// Uptime in milliseconds
    /// </summary>
    public long UptimeMs { get; set; }
}
