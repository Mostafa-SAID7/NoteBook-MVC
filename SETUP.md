# Setup Guide

Complete setup for local development and Docker deployment.

## 🐳 Docker (Recommended - Fastest)

### Option 1: Use Pre-built Image from Docker Hub
```bash
# Pull and run
docker pull msaid356/notebook:latest
docker-compose up -d

# Access at http://localhost:5000
```

### Option 2: Build from Source
```bash
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
docker-compose -f docker-compose.build.yml up -d
```

### Docker Hub Information
- **Username:** `msaid356`
- **Password:** `Memo@3560`
- **Image:** `msaid356/notebook`
- **Registry:** https://hub.docker.com/r/msaid356/notebook

### Docker Commands
```bash
# Build and push to Docker Hub
docker build -t msaid356/notebook:latest .
docker push msaid356/notebook:latest

# Run locally
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

## 💻 Local Development

### Prerequisites
- .NET 9 SDK: https://dotnet.microsoft.com/download
- PostgreSQL 16: https://www.postgresql.org/download/

### Steps

1. **Clone repository**
   ```bash
   git clone https://github.com/yourusername/NoteBook.git
   cd NoteBook
   dotnet restore
   ```

2. **Setup database**
   ```bash
   psql -U postgres -c "CREATE DATABASE notebook_db;"
   psql -U postgres -d notebook_db -f db/schema.sql
   ```

3. **Run application**
   ```bash
   dotnet build
   cd NoteBook.Web
   dotnet run
   ```

Application running at: **http://localhost:5000**

## ✅ Verify Installation

```bash
# Health check
curl http://localhost:5000/health

# Test API
curl http://localhost:5000/api/notes
```

## 🐛 Common Issues

**PostgreSQL not running?**
```bash
# Check: psql -U postgres -c "SELECT 1;"
```

**Port 5000 in use?**
```bash
# Change port in docker-compose.yml or appsettings.json
```

**Database connection failed?**
```bash
# Check connection string in appsettings.json
# Default: Host=localhost;Port=5432;Database=notebook_db;Username=postgres;Password=postgres
```

## 📚 Next Steps

- Test API endpoints: [docs/API.md](docs/API.md)
- Understand architecture: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- Deploy to production: [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)

---

**Need help?** Check [docs/INDEX.md](docs/INDEX.md) for all documentation
