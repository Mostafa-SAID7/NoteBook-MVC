namespace NoteBook.Web.Configuration;

using AspNetCoreRateLimit;

/// <summary>
/// Rate limiting configuration
/// Implements IP-based rate limiting to protect API endpoints
/// </summary>
public static class RateLimitingConfiguration
{
    /// <summary>
    /// Register rate limiting services and policies
    /// </summary>
    public static void AddRateLimiting(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();

        builder.Services.Configure<IpRateLimitOptions>(options =>
        {
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule { Endpoint = "*", Period = "1m", Limit = 100 },
                new RateLimitRule { Endpoint = "*auth*", Period = "1m", Limit = 10 },
                new RateLimitRule { Endpoint = "*search*", Period = "1m", Limit = 50 },
                new RateLimitRule { Endpoint = "*health*", Period = "1m", Limit = 1000 }
            };

            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.ClientIdHeader = "X-Client-ID";
            options.EnableEndpointRateLimiting = true;
        });

        builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

    /// <summary>
    /// Apply rate limiting middleware to the pipeline
    /// </summary>
    public static void UseRateLimiting(WebApplication app)
    {
        app.UseIpRateLimiting();
    }
}
