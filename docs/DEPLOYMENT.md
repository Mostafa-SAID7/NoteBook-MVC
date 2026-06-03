# Deployment Guide

Complete guide to deploying NoteBook to production environments.

## Pre-Deployment Checklist

- [ ] Code reviewed and merged to main
- [ ] All tests passing
- [ ] Database schema finalized
- [ ] Environment configuration prepared
- [ ] Backups configured
- [ ] Monitoring setup
- [ ] Security audit completed
- [ ] Documentation updated

## Docker Deployment

### Prerequisites

- Docker 20.10+
- Docker Compose 2.0+
- PostgreSQL connection details

### Building Docker Image

```bash
# Build image
docker build -t notebook:latest .

# Tag for registry
docker tag notebook:latest your-registry/notebook:1.0.0

# Push to registry
docker push your-registry/notebook:1.0.0
```

### Docker Compose Deployment

```bash
# Start services
docker-compose up -d

# View logs
docker-compose logs -f notebook-api

# Check health
curl http://localhost:5000/health

# Stop services
docker-compose down

# Backup volumes
docker-compose exec postgres pg_dump -U postgres notebook_db > backup.sql
```

### Docker Network Security

```yaml
# docker-compose.yml
networks:
  notebook-network:
    driver: bridge
    ipam:
      config:
        - subnet: 172.20.0.0/16
```

## Cloud Deployment

### Azure Container Instances

```bash
# Create resource group
az group create --name notebook-rg --location eastus

# Create container
az container create \
  --resource-group notebook-rg \
  --name notebook \
  --image your-registry/notebook:latest \
  --ports 80 443 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Server=..." \
  --restart-policy OnFailure

# View logs
az container logs --resource-group notebook-rg --name notebook --follow

# Delete
az container delete --resource-group notebook-rg --name notebook
```

### AWS ECS

```bash
# Create task definition
aws ecs register-task-definition \
  --family notebook \
  --container-definitions file://task-definition.json \
  --requires-compatibilities FARGATE \
  --network-mode awsvpc \
  --cpu 256 \
  --memory 512

# Create service
aws ecs create-service \
  --cluster notebook-cluster \
  --service-name notebook-service \
  --task-definition notebook \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxx],securityGroups=[sg-xxx]}"
```

### Kubernetes (Helm)

```bash
# Create namespace
kubectl create namespace notebook

# Deploy using Helm chart
helm install notebook ./chart \
  --namespace notebook \
  --values values-prod.yaml

# Check deployment
kubectl get pods -n notebook
kubectl logs -f deployment/notebook -n notebook

# Scale deployment
kubectl scale deployment notebook --replicas=3 -n notebook
```

## Environment Configuration

### Production appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.example.com;Port=5432;Database=notebook_prod;Username=notebook_user;Password=${DB_PASSWORD};SslMode=Require"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/notebook/app-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  },
  "AllowedHosts": "api.example.com,www.example.com"
}
```

### Environment Variables

```bash
# Database
CONNECTIONSTRINGS_DEFAULTCONNECTION="Host=db.example.com;..."

# Application
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="https://+:443"

# Logging
Logging__LogLevel__Default="Information"

# Security
ASPNETCORE_HTTPS_PORT=443
ASPNETCORE_ForwardedHeadersEnabled=true
```

### Secrets Management

#### Using Environment Variables
```bash
export DB_PASSWORD="your-secure-password"
```

#### Using Docker Secrets
```bash
echo "your-secure-password" | docker secret create db_password -
```

#### Using Azure Key Vault
```bash
az keyvault secret set --vault-name notebook-kv --name db-password --value "your-password"
```

#### Using AWS Secrets Manager
```bash
aws secretsmanager create-secret --name notebook/db-password --secret-string "your-password"
```

## Database Deployment

### PostgreSQL Backup

```bash
# Create backup
pg_dump -h db.example.com -U notebook_user -d notebook_db > backup_$(date +%Y%m%d).sql

# Compress
gzip backup_*.sql

# Restore from backup
psql -h db.example.com -U notebook_user -d notebook_db < backup_20260603.sql
```

### Schema Migration

1. **Backup existing database**
   ```bash
   pg_dump -h production_host -U postgres -d notebook_db > pre_migration_backup.sql
   ```

2. **Apply schema changes**
   ```bash
   psql -h production_host -U postgres -d notebook_db -f migrations/migration.sql
   ```

3. **Verify migration**
   ```bash
   psql -h production_host -U postgres -d notebook_db -c "\d notes"
   ```

### Replication Setup (High Availability)

```bash
# On primary
CREATE ROLE replication_user WITH REPLICATION ENCRYPTED PASSWORD 'password';

# On replica
pg_basebackup -h primary_host -D /var/lib/postgresql/data -U replication_user
```

## Load Balancer Configuration

### Nginx

```nginx
upstream notebook {
    server app1:5000;
    server app2:5000;
    server app3:5000;
}

server {
    listen 443 ssl http2;
    server_name api.example.com;

    ssl_certificate /etc/ssl/certs/cert.pem;
    ssl_certificate_key /etc/ssl/private/key.pem;

    location / {
        proxy_pass http://notebook;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /health {
        proxy_pass http://notebook;
        access_log off;
    }
}
```

### HAProxy

```haproxy
frontend api_frontend
    bind :443 ssl crt /path/to/cert.pem
    default_backend notebook_backend

backend notebook_backend
    balance roundrobin
    server app1 app1:5000 check
    server app2 app2:5000 check
    server app3 app3:5000 check
```

## SSL/TLS Certificate Management

### Let's Encrypt (Free)

```bash
# Using Certbot
certbot certonly --standalone -d api.example.com

# Auto-renewal
certbot renew --quiet --no-eff-email --agree-tos
```

### Self-Signed (Development)

```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes
```

## Monitoring & Logging

### Application Monitoring

**Serilog Configuration**:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://seq.example.com:5341")  // Centralized logging
    .CreateLogger();
```

### Health Checks

Add to `Program.cs`:
```csharp
app.MapHealthChecks("/health");
```

Test:
```bash
curl http://localhost:5000/health
```

### Application Performance Monitoring (APM)

#### Using Application Insights

```csharp
services.AddApplicationInsightsTelemetry(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
```

#### Using Datadog

```bash
docker run -e DD_AGENT_HOST=localhost \
  -e DD_TRACE_AGENT_PORT=8126 \
  notebook:latest
```

## Scaling Strategies

### Horizontal Scaling

1. **Stateless Application** ✓ (No session state)
2. **Distributed Database** (Read replicas, sharding)
3. **Load Balancing** (Round-robin, least connections)
4. **Caching Layer** (Redis, MemoryCache)

### Auto-Scaling Rules

```yaml
# Kubernetes HPA
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: notebook-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: notebook
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

## Disaster Recovery

### Backup Strategy

- **Frequency**: Daily at 2 AM UTC
- **Retention**: 30 days
- **Location**: Off-site (S3, Azure Blob)

```bash
#!/bin/bash
# Daily backup script
DATE=$(date +%Y%m%d_%H%M%S)
pg_dump -h db.example.com -U notebook_user -d notebook_db | \
  gzip | \
  aws s3 cp - s3://notebook-backups/db_backup_$DATE.sql.gz
```

### Recovery Procedures

1. **Database Corruption**
   - Stop application
   - Restore from latest backup
   - Run consistency check
   - Start application

2. **Full Data Loss**
   - Restore database from backup
   - Verify application connectivity
   - Run health checks

## Security Hardening

### Network Security

```bash
# Firewall rules
- Allow 443 (HTTPS) from internet
- Allow 5432 (PostgreSQL) from app servers only
- Deny all other inbound
```

### Application Security

- [ ] Disable debug mode in production
- [ ] Enable HTTPS only
- [ ] Set secure HTTP headers
- [ ] Implement rate limiting
- [ ] Validate all inputs
- [ ] Use parameterized queries (Dapper)
- [ ] Implement authentication/authorization
- [ ] Enable CORS only for trusted domains

### Database Security

- [ ] Use strong passwords
- [ ] Create non-root user for application
- [ ] Enable SSL for connections
- [ ] Regular backups encrypted
- [ ] Audit logging enabled
- [ ] Regular security updates

## Post-Deployment Verification

```bash
# Health check
curl -v https://api.example.com/health

# API test
curl https://api.example.com/api/notes

# Database connectivity
psql -h db.example.com -U notebook_user -d notebook_db -c "SELECT COUNT(*) FROM notes;"

# SSL certificate
echo | openssl s_client -connect api.example.com:443 | grep -A 1 "Subject:"

# Performance test
ab -n 1000 -c 10 https://api.example.com/api/notes
```

## Monitoring Dashboard

**Key Metrics to Monitor**:
- Request latency (p50, p95, p99)
- Error rate
- Database connection pool usage
- CPU/Memory usage
- Disk I/O
- Network throughput

## Rollback Procedure

1. **Identify Issue**
   - Check logs for errors
   - Monitor alert thresholds

2. **Prepare Rollback**
   ```bash
   docker pull your-registry/notebook:1.0.0-previous
   ```

3. **Execute Rollback**
   ```bash
   docker-compose down
   # Update docker-compose.yml with previous version
   docker-compose up -d
   ```

4. **Verify**
   ```bash
   curl http://localhost:5000/health
   ```

## Maintenance Windows

Schedule during low-traffic hours:
- Database updates
- Certificate renewals
- System patches
- Infrastructure changes

---

**Version**: 1.0.0  
**Last Updated**: June 2026
