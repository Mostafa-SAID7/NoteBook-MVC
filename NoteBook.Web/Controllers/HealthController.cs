namespace NoteBook.Web.Controllers;

using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteBook.Application.DTOs;
using NoteBook.Infrastructure.Data;

/// <summary>
/// Health check endpoint for monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Health checks should be accessible without authentication
public class HealthController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IWebHostEnvironment _environment;
    private static readonly DateTime StartTime = DateTime.UtcNow;

    public HealthController(IDbConnectionFactory connectionFactory, IWebHostEnvironment environment)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    /// <summary>
    /// Get health status of the API and its dependencies
    /// </summary>
    /// <returns>Health check response</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthCheckResponse>> Get()
    {
        var response = new HealthCheckResponse
        {
            Service = "NoteBook API",
            Timestamp = DateTime.UtcNow,
            Version = "2.0.0",
            Environment = _environment.EnvironmentName,
            UptimeMs = (long)(DateTime.UtcNow - StartTime).TotalMilliseconds
        };

        // Check database connectivity
        response.DatabaseConnected = await CheckDatabaseConnectivity();

        // Determine overall status
        if (!response.DatabaseConnected)
        {
            response.Status = "Unhealthy";
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        response.Status = "Healthy";
        return Ok(response);
    }

    /// <summary>
    /// Quick liveness probe for orchestrators (Kubernetes, Docker Swarm)
    /// </summary>
    /// <returns>Simple OK response</returns>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live()
    {
        return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Readiness probe to check if service is ready to accept traffic
    /// </summary>
    /// <returns>OK if ready, 503 if not ready</returns>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready()
    {
        var isReady = await CheckDatabaseConnectivity();
        
        if (!isReady)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                new { status = "not_ready", reason = "Database unavailable" });
        }

        return Ok(new { status = "ready", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Check if database is accessible
    /// </summary>
    private async Task<bool> CheckDatabaseConnectivity()
    {
        try
        {
            using var connection = _connectionFactory.GetConnection();
            const string query = "SELECT 1;";
            var result = await connection.QuerySingleOrDefaultAsync<int>(query);
            return result == 1;
        }
        catch
        {
            return false;
        }
    }
}
