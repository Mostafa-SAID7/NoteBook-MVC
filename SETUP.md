# Setup Guide

Quick start for local development and Docker deployment.

## 🐳 Docker (Fastest)

```bash
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
docker-compose up --build
```

Application running at: **http://localhost:5000**

> Includes PostgreSQL, application, and schema initialization

## 💻 Local Development

### Prerequisites
- .NET 9 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
- PostgreSQL 16: https://www.postgresql.org/download/

### Setup Steps

1. **Clone & restore**
   ```bash
   git clone https://github.com/yourusername/NoteBook.git
   cd NoteBook
   dotnet restore
   ```

2. **Setup PostgreSQL**
   ```bash
   # Create database
   psql -U postgres -c "CREATE DATABASE notebook_db;"
   
   # Initialize schema
   psql -U postgres -d notebook_db -f db/schema.sql
   ```

3. **Configure connection** (if needed)
   - Edit: `NoteBook.Web/appsettings.json`
   - Connection string already uses defaults (postgres:postgres)

4. **Build & run**
   ```bash
   dotnet build
   cd NoteBook.Web
   dotnet run
   ```

Application running at: **http://localhost:5000**

## 📝 Configuration

### Connection String
File: `NoteBook.Web/appsettings.json`

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=notebook_db;Username=postgres;Password=postgres"
```

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5000
```

## ✅ Verify Installation

```bash
# Test API
curl http://localhost:5000/api/notes

# Should return: [] (empty array)
```

## 🐛 Troubleshooting

**Connection refused?**
- Ensure PostgreSQL is running: `psql -U postgres -c "SELECT 1;"`
- Check port 5432 availability

**Database doesn't exist?**
- Create it: `psql -U postgres -c "CREATE DATABASE notebook_db;"`
- Initialize: `psql -U postgres -d notebook_db -f db/schema.sql`

**Port 5000 in use?**
- Use different port: `export ASPNETCORE_URLS=http://+:5001`

See [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) for more issues.

## 📚 Next Steps

- [API Documentation](docs/API.md)
- [Architecture Guide](docs/ARCHITECTURE.md)
- [Development Workflow](docs/DEVELOPMENT.md)
- [All Documentation](docs/INDEX.md)

---

**Need help?** Check [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)
