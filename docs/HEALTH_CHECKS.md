# Health Check Endpoints

## Overview

NoteBook API provides health check endpoints for monitoring service availability and dependencies. These endpoints are useful for:

- Kubernetes liveness/readiness probes
- Docker container health checks
- Load balancer health monitoring
- Uptime monitoring services
- Diagnostic tools

---

## Endpoints

### 1. Full Health Check

**Endpoint**: `GET /api/health`

**Authentication**: Not required (publicly accessible)

**Purpose**: Comprehensive health status with dependency checks

**Response (Healthy - 200 OK)**:
```json
{
  "status": "Healthy",
  "service": "NoteBook API",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "2.0.0",
  "environment": "Production",
  "databaseConnected": true,
  "uptimeMs": 3600000
}
```

**Response (Unhealthy - 503 Service Unavailable)**:
```json
{
  "status": "Unhealthy",
  "service": "NoteBook API",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "2.0.0",
  "environment": "Production",
  "databaseConnected": false,
  "uptimeMs": 3600000
}
```

**Status Codes**:
- `200 OK` - All checks passed
- `503 Service Unavailable` - One or more checks failed

---

### 2. Liveness Probe

**Endpoint**: `GET /api/health/live`

**Authentication**: Not required

**Purpose**: Quick check that the process is alive (Kubernetes/Docker)

**Response (200 OK)**:
```json
{
  "status": "alive",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Use Case**: Kubernetes `livenessProbe` configuration

```yaml
# Kubernetes manifest
livenessProbe:
  httpGet:
    path: /api/health/live
    port: 5000
  initialDelaySeconds: 10
  periodSeconds: 5
  failureThreshold: 3
```

---

### 3. Readiness Probe

**Endpoint**: `GET /api/health/ready`

**Authentication**: Not required

**Purpose**: Check if service is ready to accept traffic

**Response (Ready - 200 OK)**:
```json
{
  "status": "ready",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Response (Not Ready - 503 Service Unavailable)**:
```json
{
  "status": "not_ready",
  "reason": "Database unavailable",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Use Case**: Kubernetes `readinessProbe` configuration

```yaml
# Kubernetes manifest
readinessProbe:
  httpGet:
    path: /api/health/ready
    port: 5000
  initialDelaySeconds: 5
  periodSeconds: 10
  failureThreshold: 2
```

---

## Health Check Components

### 1. Database Connectivity

The `/api/health` endpoint checks PostgreSQL connectivity:

```csharp
// Performs a simple query: SELECT 1;
public async Task<bool> CheckDatabaseConnectivity()
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
```

**Status**: `databaseConnected: true/false`

### 2. Service Uptime

Time since application started (in milliseconds):

```json
{
  "uptimeMs": 3600000  // 1 hour
}
```

### 3. Environment Information

Current deployment environment:

```json
{
  "environment": "Development",  // or "Staging", "Production"
  "version": "2.0.0"
}
```

---

## Docker Configuration

### Docker Health Check

Add to your Dockerfile or docker-compose.yml:

```dockerfile
# In Dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:5000/api/health || exit 1
```

Or in docker-compose.yml:

```yaml
services:
  notebook-api:
    image: notebook:latest
    ports:
      - "5000:5000"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/api/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 40s
    depends_on:
      postgres:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

---

## Kubernetes Configuration

### Full Kubernetes Manifest

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: notebook-api
  labels:
    app: notebook
spec:
  replicas: 3
  selector:
    matchLabels:
      app: notebook
  template:
    metadata:
      labels:
        app: notebook
    spec:
      containers:
      - name: notebook-api
        image: notebook:2.0.0
        imagePullPolicy: Always
        ports:
        - containerPort: 5000
          name: http
        
        # Liveness: Restart if service is dead
        livenessProbe:
          httpGet:
            path: /api/health/live
            port: http
          initialDelaySeconds: 10
          periodSeconds: 5
          failureThreshold: 3
          timeoutSeconds: 2
        
        # Readiness: Only route traffic when ready
        readinessProbe:
          httpGet:
            path: /api/health/ready
            port: http
          initialDelaySeconds: 5
          periodSeconds: 10
          failureThreshold: 2
          timeoutSeconds: 2
        
        # Startup: Wait for app to start
        startupProbe:
          httpGet:
            path: /api/health/live
            port: http
          failureThreshold: 30
          periodSeconds: 1
        
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: notebook-secrets
              key: db-connection-string
        
        resources:
          requests:
            cpu: 100m
            memory: 256Mi
          limits:
            cpu: 500m
            memory: 512Mi
```

### Service Configuration

```yaml
apiVersion: v1
kind: Service
metadata:
  name: notebook-api
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 5000
    name: http
  selector:
    app: notebook
```

---

## Monitoring Examples

### Prometheus Metrics

Add Prometheus scraping to your monitoring setup:

```yaml
# prometheus.yml
scrape_configs:
  - job_name: 'notebook-api'
    static_configs:
      - targets: ['localhost:5000']
    metrics_path: '/metrics'
    scrape_interval: 15s
```

### Health Check Monitoring Script

**Bash**:
```bash
#!/bin/bash

while true; do
  response=$(curl -s -w "\n%{http_code}" http://localhost:5000/api/health)
  http_code=$(echo "$response" | tail -n 1)
  body=$(echo "$response" | sed '$d')
  
  if [ "$http_code" != "200" ]; then
    echo "❌ Health check failed: HTTP $http_code"
    echo "$body" | jq .
  else
    status=$(echo "$body" | jq -r '.status')
    uptime=$(echo "$body" | jq -r '.uptimeMs')
    echo "✅ Healthy - Uptime: ${uptime}ms"
  fi
  
  sleep 30
done
```

**Python**:
```python
import requests
import json
import time

def monitor_health(url, interval=30):
    while True:
        try:
            response = requests.get(f"{url}/api/health", timeout=5)
            
            if response.status_code == 200:
                data = response.json()
                print(f"✅ Healthy - Status: {data['status']}")
                print(f"   Database: {data['databaseConnected']}")
                print(f"   Uptime: {data['uptimeMs']}ms")
            else:
                print(f"❌ Unhealthy - HTTP {response.status_code}")
                
        except Exception as e:
            print(f"❌ Error: {e}")
        
        time.sleep(interval)

if __name__ == "__main__":
    monitor_health("http://localhost:5000")
```

---

## Client Integration

### JavaScript/Fetch

```javascript
class HealthMonitor {
  constructor(baseUrl, checkInterval = 30000) {
    this.baseUrl = baseUrl;
    this.checkInterval = checkInterval;
    this.listeners = [];
  }

  on(event, callback) {
    this.listeners.push({ event, callback });
  }

  async check() {
    try {
      const response = await fetch(`${this.baseUrl}/api/health`);
      const data = await response.json();
      
      const isHealthy = response.ok;
      this.emit('health', { isHealthy, data });
      
      return { isHealthy, data };
    } catch (error) {
      this.emit('error', error);
      return { isHealthy: false, error };
    }
  }

  emit(event, data) {
    this.listeners
      .filter(l => l.event === event)
      .forEach(l => l.callback(data));
  }

  start() {
    this.check();
    this.interval = setInterval(() => this.check(), this.checkInterval);
  }

  stop() {
    clearInterval(this.interval);
  }
}

// Usage
const monitor = new HealthMonitor('http://localhost:5000');

monitor.on('health', ({ isHealthy, data }) => {
  if (isHealthy) {
    console.log(`✅ API is healthy - Uptime: ${data.uptimeMs}ms`);
  } else {
    console.log('❌ API is unhealthy');
  }
});

monitor.on('error', (error) => {
  console.error('Health check error:', error);
});

monitor.start();
```

### C# / HttpClient

```csharp
public class HealthCheckClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HealthCheckClient> _logger;

    public HealthCheckClient(HttpClient httpClient, ILogger<HealthCheckClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HealthCheckResponse> GetHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/health");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<HealthCheckResponse>(content);
            }
            
            _logger.LogWarning("Health check failed with status {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check request failed");
            return null;
        }
    }
}
```

---

## Troubleshooting

### Database Connectivity Failed

**Symptom**: `databaseConnected: false` in health response

**Causes**:
- PostgreSQL not running
- Connection string misconfigured
- Network connectivity issue
- Database credentials invalid

**Solutions**:
```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Verify connection string in appsettings.json
cat appsettings.json | grep ConnectionString

# Test PostgreSQL connection
psql -h localhost -U postgres -d notebook -c "SELECT 1;"
```

### Health Check Returns 503

**Symptom**: `/api/health` returns HTTP 503

**Cause**: One or more health checks failed

**Solution**: 
1. Check individual probes: `/api/health/live` and `/api/health/ready`
2. Review application logs for errors
3. Verify database connectivity

---

## Best Practices

### 1. Update Frequency

- **Liveness probe**: Check every 5-10 seconds
- **Readiness probe**: Check every 10-30 seconds
- **Full health**: Check every 30-60 seconds

### 2. Timeout Values

- **Short timeout (2-3s)**: For quick liveness checks
- **Longer timeout (5-10s)**: For database-dependent checks

### 3. Failure Thresholds

- **Liveness**: 3 failures to restart (30 seconds at 10s interval)
- **Readiness**: 1-2 failures to remove from load balancer
- **Startup**: 30 attempts (allow 30s for startup)

### 4. Load Balancer Configuration

```nginx
# nginx example
upstream notebook_api {
    server localhost:5000 max_fails=2 fail_timeout=30s;
}

server {
    listen 80;
    server_name api.notebook.com;

    location / {
        proxy_pass http://notebook_api;
        proxy_connect_timeout 5s;
        proxy_send_timeout 10s;
        proxy_read_timeout 10s;
    }
}
```

---

## Monitoring Dashboard

Example Grafana dashboard query:

```promql
# Uptime gauge (hours)
notebook_api:uptime_hours = (notebook_api_uptime_ms / 1000 / 3600)

# Health status
notebook_api:health_status = notebook_api_health_check_success

# Request latency
histogram_quantile(0.95, notebook_api_request_duration_seconds_bucket)
```

---

## Related Documentation

- [DEPLOYMENT.md](DEPLOYMENT.md) - Production deployment
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Common issues
- [DEVELOPMENT.md](DEVELOPMENT.md) - Local development

---

**Last Updated**: June 2026  
**API Version**: 2.1.0
