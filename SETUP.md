# NoteBook Setup Guide

Complete step-by-step guide to get NoteBook running locally or in Docker.

## Quick Start (Docker - Recommended)

The fastest way to get everything running:

```bash
# Clone the repository
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook

# Start the application and database
docker-compose up --build

# Wait for services to start (about 30 seconds)
# Access the application at http://localhost:5000
```

That's it! Docker Compose handles:
- PostgreSQL database creation and schema initialization
- Application build and startup
- Environment configuration

## Local Development Setup

### Prerequisites

- **.NET 9 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **PostgreSQL 16** ([Download](https://www.postgresql.org/download/))
- **Visual Studio Code** or **Visual Studio 2022+** (optional but recommended)
- **Git** for version control

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
```

### Step 2: Setup PostgreSQL Database

#### Option A: Using PostgreSQL Server (Windows/Mac/Linux)

1. **Create database and user:**
   ```bash
   psql -U postgres
   ```

2. **In psql prompt:**
   ```sql
   CREATE DATABASE notebook_db;
   CREATE USER notebook_user WITH PASSWORD 'your_secure_password';
   ALTER ROLE notebook_user SET client_encoding TO 'utf8';
   ALTER ROLE notebook_user SET default_transaction_isolation TO 'read committed';
   ALTER ROLE notebook_user SET default_transaction_deferrable TO on;
   ALTER ROLE notebook_user SET default_transaction_read_only TO off;
   GRANT ALL PRIVILEGES ON DATABASE notebook_db TO notebook_user;
   ```

3. **Initialize schema:**
   ```bash
   psql -U notebook_user -d notebook_db -f database.sql
   ```

#### Option B: Using Docker (Without Full Docker Compose)

```bash
docker run --name notebook_postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=notebook_db \
  -p 5432:5432 \
  -d postgres:16-alpine

# Initialize schema
docker cp database.sql notebook_postgres:/tmp/
docker exec notebook_postgres psql -U postgres -d notebook_db -f /tmp/database.sql
```

### Step 3: Configure Application

1. **Edit `NoteBook.Web/appsettings.json`:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=notebook_db;Username=postgres;Password=postgres"
     }
   }
   ```

   Replace connection string with your PostgreSQL credentials if different.

2. **Set environment (optional):**
   ```bash
   # Linux/Mac
   export ASPNETCORE_ENVIRONMENT=Development
   
   # Windows PowerShell
   $env:ASPNETCORE_ENVIRONMENT="Development"
   
   # Windows CMD
   set ASPNETCORE_ENVIRONMENT=Development
   ```

### Step 4: Restore and Build

```bash
# Restore NuGet packages
dotnet restore

# Build solution
dotnet build

# Expected output: "Build succeeded"
```

### Step 5: Run the Application

```bash
cd NoteBook.Web
dotnet run
```

Expected output:
```
info: NoteBook application starting...
info: Now listening on: https://localhost:5001
info: Now listening on: http://localhost:5000
```

### Step 6: Verify Installation

1. **Open in browser:**
   - API: http://localhost:5000
   - HTTPS: https://localhost:5001 (requires accepting self-signed cert)

2. **Test API endpoint:**
   ```bash
   # Linux/Mac
   curl http://localhost:5000/api/notes
   
   # Windows PowerShell
   Invoke-WebRequest -Uri http://localhost:5000/api/notes
   ```

   Expected response: `[]` (empty array)

## Development Workflow

### Running Tests

```bash
# Build tests (if implemented)
dotnet test
```

### Code Changes

1. **Make changes to your code**
2. **Hot reload (automatic)** or **restart the app**:
   ```bash
   # Ctrl+C to stop
   dotnet run  # Restart
   ```

### Database Changes

1. **Update `database.sql` with new schema**
2. **Recreate database:**
   ```bash
   # Drop existing
   psql -U postgres -c "DROP DATABASE notebook_db;"
   
   # Create new
   psql -U postgres -c "CREATE DATABASE notebook_db;"
   
   # Initialize
   psql -U postgres -d notebook_db -f database.sql
   ```

## Docker Deployment

### Build Docker Image

```bash
docker build -t notebook:latest .
```

### Run Single Container

```bash
docker run -d \
  --name notebook \
  -p 5000:5000 \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=notebook_db;Username=postgres;Password=postgres" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  notebook:latest
```

### Docker Compose (Full Stack)

```bash
# Start services
docker-compose up -d

# View logs
docker-compose logs -f notebook-api

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Troubleshooting

### Issue: Connection Refused (PostgreSQL)

**Solution:**
```bash
# Verify PostgreSQL is running
psql -U postgres -c "SELECT version();"

# Check port 5432 is listening
netstat -tuln | grep 5432  # Linux
lsof -i :5432              # Mac
```

### Issue: Database Not Initialized

**Solution:**
```bash
# Manually run schema script
psql -U postgres -d notebook_db -f database.sql

# Verify tables exist
psql -U postgres -d notebook_db -c "\dt"
```

### Issue: Port 5000 Already in Use

**Solution:**
```bash
# Use different port
# Windows PowerShell
$env:ASPNETCORE_URLS="http://+:5001"

# Linux/Mac
export ASPNETCORE_URLS="http://+:5001"

# Then run
dotnet run
```

### Issue: Dependencies Not Restoring

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore --force
```

### Issue: Docker Container Won't Start

**Solution:**
```bash
# Check logs
docker-compose logs notebook-api

# Rebuild images
docker-compose build --no-cache

# Start fresh
docker-compose down -v
docker-compose up --build
```

## Environment Variables

Common environment variables for configuration:

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Execution environment | `Development`, `Production` |
| `ASPNETCORE_URLS` | Server URLs | `http://+:5000` |
| `ConnectionStrings__DefaultConnection` | Database connection | `Host=localhost;Port=5432;...` |
| `Logging__LogLevel__Default` | Default log level | `Information`, `Debug` |

## Next Steps

1. **Review Architecture**: Read `README.md` for architecture overview
2. **Explore API**: Use a tool like [Postman](https://www.postman.com/) to test endpoints
3. **Add Features**: Follow the development guidelines in `README.md`
4. **Setup Authentication**: Implement user auth before production use
5. **Deploy**: Use Docker or deploy to your hosting platform

## Production Deployment Checklist

- [ ] Configure production connection string
- [ ] Implement authentication (JWT, Identity, etc.)
- [ ] Setup HTTPS/SSL certificates
- [ ] Configure logging aggregation (Seq, ELK, etc.)
- [ ] Setup database backups
- [ ] Configure firewall rules
- [ ] Setup monitoring and alerts
- [ ] Review security settings
- [ ] Load test the application
- [ ] Create deployment documentation

## Support

For issues:
1. Check this guide first
2. Review logs: `logs/app-*.txt`
3. Check GitHub Issues
4. Create detailed issue report

---

**Version**: 1.0.0  
**Last Updated**: June 2026
