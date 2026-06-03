# Rate Limiting Guide

Comprehensive guide to the NoteBook API rate limiting feature for protecting endpoints from abuse and ensuring fair usage.

---

## Overview

Rate limiting restricts the number of API requests a client can make within a specified time period. This protects the API from:
- ✅ Brute force attacks
- ✅ API abuse and DoS attacks
- ✅ Resource exhaustion
- ✅ Fair usage enforcement

---

## Default Rate Limits

### By Endpoint

| Endpoint | Limit | Period | Purpose |
|----------|-------|--------|---------|
| All endpoints (`*`) | 100 | 1 minute | General default |
| Auth endpoints (`*auth*`) | 10 | 1 minute | Brute force prevention |
| Search endpoints (`*search*`) | 50 | 1 minute | Query optimization |
| Health endpoints (`*health*`) | 1000 | 1 minute | Monitoring friendly |

### Examples

```bash
# ✅ Allowed - Within limit
curl http://localhost:5000/api/notes
# HTTP 200 OK

# ✅ Allowed - First 10 attempts
curl http://localhost:5000/api/auth/login \
  -d '{"username":"user","password":"pass"}'
# HTTP 200 OK

# ❌ Blocked - Exceeds 10 per minute
curl http://localhost:5000/api/auth/login \
  -d '{"username":"user","password":"wrong"}'
# HTTP 429 Too Many Requests
# Retry-After: 60
```

---

## HTTP Response

### Success Response
```
HTTP 200 OK
Content-Type: application/json
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 99
X-RateLimit-Reset: 1234567890
```

### Rate Limit Exceeded
```
HTTP 429 Too Many Requests
Content-Type: application/json
Retry-After: 60

{
  "message": "Rate limit exceeded. Maximum 100 requests per minute allowed.",
  "timestamp": "2026-06-03T10:30:00Z"
}
```

**Status Code**: `429 Too Many Requests`  
**Retry-After**: `60` (seconds until limit resets)

---

## Configuration

### appsettings.json

```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-Client-ID",
    "HttpStatusCode": 429,
    "IpWhitelist": [],
    "EndpointWhitelist": [
      "*:/api/health*"
    ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "*auth*",
        "Period": "1m",
        "Limit": 10
      },
      {
        "Endpoint": "*search*",
        "Period": "1m",
        "Limit": 50
      }
    ]
  }
}
```

### Configuration Options

| Option | Value | Description |
|--------|-------|-------------|
| `EnableEndpointRateLimiting` | `true` | Enable per-endpoint limits |
| `StackBlockedRequests` | `false` | Don't queue blocked requests |
| `RealIpHeader` | `X-Real-IP` | Header for proxy IP |
| `ClientIdHeader` | `X-Client-ID` | Header for client ID |
| `HttpStatusCode` | `429` | Response status code |

### Rule Options

```json
{
  "Endpoint": "*auth*",     // Wildcard pattern to match
  "Period": "1m",           // Time period (1s, 1m, 1h)
  "Limit": 10               // Maximum requests in period
}
```

**Period Formats:**
- `1s` - 1 second
- `1m` - 1 minute (default)
- `1h` - 1 hour

---

## Customization

### Whitelist IPs

Allow specific IPs to bypass rate limits:

```json
{
  "IpRateLimiting": {
    "IpWhitelist": [
      "127.0.0.1",
      "::1",
      "192.168.1.100"
    ]
  }
}
```

### Whitelist Endpoints

Exclude specific endpoints from rate limiting:

```json
{
  "IpRateLimiting": {
    "EndpointWhitelist": [
      "*:/api/health*",
      "GET:/api/notes"
    ]
  }
}
```

### Add Custom Rules

Add or modify rules in `appsettings.json`:

```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "*payments*",
        "Period": "1h",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/notes",
        "Period": "1m",
        "Limit": 20
      }
    ]
  }
}
```

### Environment-Specific Configuration

**appsettings.Development.json:**
```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 9999
      }
    ]
  }
}
```

**appsettings.Production.json:**
```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "*auth*",
        "Period": "1m",
        "Limit": 5
      }
    ]
  }
}
```

---

## Testing Rate Limits

### Bash Script

```bash
#!/bin/bash

echo "Testing rate limits..."

# Test general endpoint (100/min limit)
for i in {1..101}; do
  echo "Request $i..."
  response=$(curl -s -w "\n%{http_code}" http://localhost:5000/api/notes)
  http_code=$(echo "$response" | tail -n 1)
  
  if [ "$http_code" == "429" ]; then
    echo "✅ Rate limited on request $i"
    break
  fi
  
  if [ $i -eq 101 ]; then
    echo "❌ Not rate limited"
  fi
  
  sleep 0.01  # Slight delay
done

# Test auth endpoint (10/min limit)
echo ""
echo "Testing auth endpoint..."
for i in {1..11}; do
  echo "Auth request $i..."
  response=$(curl -s -w "\n%{http_code}" -X POST http://localhost:5000/api/auth/login)
  http_code=$(echo "$response" | tail -n 1)
  
  if [ "$http_code" == "429" ]; then
    echo "✅ Auth rate limited on request $i"
    break
  fi
  
  sleep 0.01
done
```

### PowerShell Script

```powershell
# Test rate limiting
$uri = "http://localhost:5000/api/notes"
$requests = 0

for ($i = 1; $i -le 105; $i++) {
    try {
        $response = Invoke-WebRequest -Uri $uri -ErrorAction Stop
        $requests++
        Write-Host "✅ Request $i - OK ($($response.StatusCode))"
    }
    catch [System.Net.WebException] {
        if ($_.Exception.Response.StatusCode -eq 429) {
            Write-Host "⏸️ Rate limited on request $i!"
            Write-Host "   Retry-After: $($_.Exception.Response.Headers['Retry-After']) seconds"
            break
        }
    }
    
    Start-Sleep -Milliseconds 10
}

Write-Host "Completed $requests requests before hitting limit"
```

### Using curl

```bash
# Simple test - make 105 requests
for i in {1..105}; do
  curl -s http://localhost:5000/api/notes | head -1
done | grep -c '"title"'
# Should show ~100 successful responses, then 429s
```

---

## Client-Side Handling

### JavaScript

```javascript
async function callApi(url, options = {}) {
  try {
    const response = await fetch(url, options);
    
    if (response.status === 429) {
      const retryAfter = response.headers.get('Retry-After');
      const waitSeconds = parseInt(retryAfter || 60);
      
      console.warn(`Rate limited. Retry after ${waitSeconds}s`);
      
      // Wait before retrying
      await new Promise(resolve => 
        setTimeout(resolve, waitSeconds * 1000)
      );
      
      // Retry request
      return callApi(url, options);
    }
    
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return await response.json();
    
  } catch (error) {
    console.error('API call failed:', error);
    throw error;
  }
}

// Usage
try {
  const notes = await callApi('http://localhost:5000/api/notes');
  console.log(notes);
} catch (error) {
  console.error('Failed to get notes:', error);
}
```

### C# / HttpClient

```csharp
public class RateLimitAwareHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RateLimitAwareHttpClient> _logger;

    public RateLimitAwareHttpClient(HttpClient httpClient, ILogger<RateLimitAwareHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string url)
    {
        int retries = 3;
        int delay = 1000; // milliseconds

        while (retries > 0)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta?.TotalMilliseconds 
                        ?? delay;
                    
                    _logger.LogWarning($"Rate limited. Waiting {retryAfter}ms...");
                    
                    await Task.Delay((int)retryAfter);
                    retries--;
                    continue;
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Request failed: {ex.Message}");
                throw;
            }
        }

        throw new InvalidOperationException("Max retries exceeded");
    }
}
```

---

## Monitoring & Analytics

### Check Current Limits

Add monitoring endpoint (future enhancement):

```csharp
[HttpGet("rate-limit-status")]
[AllowAnonymous]
public IActionResult GetRateLimitStatus()
{
    // Return current limits and usage for client IP
    return Ok(new 
    { 
        remaining = 98,
        limit = 100,
        resetAt = "2026-06-03T10:31:00Z"
    });
}
```

### Log Rate Limit Events

Rate limiting events are logged automatically via Serilog:

```
2026-06-03 10:30:45 [WARN] Rate limit exceeded for IP 127.0.0.1
2026-06-03 10:30:46 [WARN] Rate limit exceeded for IP 192.168.1.100
```

---

## Best Practices

### ✅ DO

- ✅ Set stricter limits for sensitive endpoints (auth, payments)
- ✅ Set looser limits for read-only endpoints (GET)
- ✅ Monitor and adjust based on actual usage patterns
- ✅ Whitelist internal/trusted IPs
- ✅ Use consistent time periods (prefer minutes)
- ✅ Implement exponential backoff in clients
- ✅ Return meaningful error messages
- ✅ Log rate limit violations

### ❌ DON'T

- ❌ Set too-strict limits (breaks legitimate usage)
- ❌ Set too-loose limits (doesn't protect API)
- ❌ Block health checks or monitoring
- ❌ Change limits frequently
- ❌ Ignore rate limit violations in logs
- ❌ Use rate limiting as primary security (also use auth, CORS)
- ❌ Forget to test with realistic traffic patterns

---

## Production Considerations

### Load Balancer Integration

With load balancers, use `X-Real-IP` header:

```nginx
# nginx.conf
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
```

### Redis-Based Rate Limiting (Future)

For distributed deployments:

```bash
# Add Redis service
docker run -d -p 6379:6379 redis:7-alpine

# Switch to Redis store (future implementation)
services.AddStackExchangeRedisCache(options => 
    options.Configuration = "localhost:6379");
```

### Metrics & Dashboards

Track in monitoring system:
- Requests per IP per minute
- Rate limit hits over time
- Average response time
- Blocked IPs trending

---

## Troubleshooting

### I'm getting 429 responses immediately

**Problem**: Rate limit too strict for your use case

**Solution**: 
1. Check current limits in `appsettings.json`
2. Increase `Limit` value
3. Increase `Period` to "5m" or "1h"
4. Whitelist your IP if trusted
5. Restart application

### Rate limiting not working

**Problem**: Middleware not registered properly

**Solution**:
1. Verify `app.UseIpRateLimiting()` called in Program.cs
2. Check that AspNetCoreRateLimit package is installed
3. Verify configuration in appsettings.json
4. Check application logs for errors

### Health checks are blocked

**Problem**: Health endpoint hitting rate limit

**Solution**: 
1. Add to endpoint whitelist:
   ```json
   "EndpointWhitelist": ["*:/api/health*"]
   ```
2. Or increase health endpoint limit to 1000+

---

## Performance Impact

- **Per-Request Overhead**: ~1-2ms
- **Memory Usage**: ~1MB (MemoryCache)
- **Storage**: In-memory counters only
- **Scalability**: Single-server only (use Redis for distributed)

---

## API Endpoint Status

| Endpoint | Limit | Status |
|----------|-------|--------|
| `/api/notes` | 100/min | ✅ Protected |
| `/api/notes/{id}` | 100/min | ✅ Protected |
| `/api/notes/search` | 50/min | ✅ Protected |
| `/api/auth/login` | 10/min | ✅ Protected |
| `/api/health*` | 1000/min | ✅ Monitoring friendly |

---

## Related Documentation

- [API.md](API.md) - Full API documentation
- [DEPLOYMENT.md](DEPLOYMENT.md) - Production deployment
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Common issues
- [ADVANCED_FEATURES_REVIEW.md](../ADVANCED_FEATURES_REVIEW.md) - Feature overview

---

**Last Updated**: June 3, 2026  
**Feature**: Rate Limiting  
**Status**: ✅ Implemented (Phase 5)  
**Package**: AspNetCoreRateLimit v5.0.0
