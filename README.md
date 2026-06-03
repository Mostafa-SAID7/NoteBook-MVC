# NoteBook - Production-Grade Note-Taking Application

A modern, clean architecture-based note-taking application built with ASP.NET Core 9, PostgreSQL, and Dapper.

## Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
NoteBook.Domain/          # Core business entities and interfaces
  ├── Entities/           # Note, ApplicationUser, Tag
  ├── Exceptions/         # Domain-specific exceptions
  └── Repositories/       # Repository interfaces

NoteBook.Application/     # Application logic and orchestration
  ├── Features/           # MediatR Commands/Queries and Handlers
  ├── DTOs/               # Data Transfer Objects
  └── Mapping/            # AutoMapper profiles

NoteBook.Infrastructure/  # External concerns and data access
  ├── Data/               # Database connection factory
  └── Repositories/       # Dapper-based repository implementations

NoteBook.Web/            # ASP.NET Core presentation layer
  ├── Controllers/        # API controllers
  ├── Views/              # Razor views (if using MVC)
  └── Models/             # View models
```

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Language**: C# (with nullable reference types enabled)
- **Database**: PostgreSQL 16
- **Data Access**: Dapper (micro-ORM) + Npgsql
- **Architecture Pattern**: CQRS with MediatR
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Containerization**: Docker & Docker Compose

## Features

### Core Functionality
- ✅ Create, read, update, and soft delete notes
- ✅ Rich text content support
- ✅ Tag-based categorization
- ✅ Full-text search (title, content, tags)
- ✅ Note archiving
- ✅ Soft delete with restore capability
- ✅ Pagination and sorting

### Non-Functional Requirements
- ✅ Clean, layered architecture
- ✅ Dependency injection with Microsoft.Extensions.DependencyInjection
- ✅ Structured logging with Serilog
- ✅ PostgreSQL with proper indexing
- ✅ Async/await throughout
- ✅ CancellationToken support
- ✅ RESTful API design
- ✅ Docker containerization
- ✅ Health checks

## Prerequisites

- .NET 9 SDK
- PostgreSQL 16 (or Docker)
- Visual Studio Code or Visual Studio 2022+

## Getting Started

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/NoteBook.git
   cd NoteBook
   ```

2. **Configure PostgreSQL connection**
   - Edit `NoteBook.Web/appsettings.json`
   - Update the `DefaultConnection` string with your PostgreSQL credentials:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=notebook_db;Username=postgres;Password=yourpassword"
   }
   ```

3. **Create the database and schema**
   ```bash
   # Using psql
   psql -U postgres -f db/schema.sql
   
   # Or execute the SQL script in your database client
   ```

4. **Restore dependencies and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run the application**
   ```bash
   cd NoteBook.Web
   dotnet run
   ```
   
   The application will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

### Docker Setup

1. **Using Docker Compose (recommended)**
   ```bash
   docker-compose up --build
   ```
   
   This starts both PostgreSQL and the NoteBook application:
   - API: http://localhost:5000
   - PostgreSQL: localhost:5432

2. **Using individual Docker commands**
   ```bash
   # Build the image
   docker build -t notebook:latest .
   
   # Run the container
   docker run -d \
     -p 5000:5000 \
     -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=notebook_db;Username=postgres;Password=postgres" \
     --name notebook \
     notebook:latest
   ```

## API Endpoints

### Notes Management

- **GET /api/notes** - Get all notes for the current user
- **GET /api/notes/{id}** - Get a specific note
- **POST /api/notes** - Create a new note
  ```json
  {
    "title": "My Note",
    "content": "Note content here",
    "tags": "important,work"
  }
  ```
- **PUT /api/notes/{id}** - Update a note
- **DELETE /api/notes/{id}** - Delete a note (soft delete)
- **GET /api/notes/search?term=searchterm** - Search notes

## Database Schema

The application uses the following tables:

### application_users
```sql
- id (UUID, PK)
- email (VARCHAR 255, UNIQUE)
- user_name (VARCHAR 255, UNIQUE)
- password_hash (VARCHAR 255)
- full_name (VARCHAR 255)
- created_at (TIMESTAMP)
- is_active (BOOLEAN)
- last_login_at (TIMESTAMP, NULL)
```

### notes
```sql
- id (UUID, PK)
- title (VARCHAR 255)
- content (TEXT)
- tags (TEXT)
- user_id (UUID, FK → application_users)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
- is_deleted (BOOLEAN) - for soft delete
- deleted_at (TIMESTAMP, NULL)
- is_archived (BOOLEAN)
- archived_at (TIMESTAMP, NULL)
```

### tags
```sql
- id (UUID, PK)
- name (VARCHAR 255)
- user_id (UUID, FK → application_users)
- created_at (TIMESTAMP)
- usage_count (INTEGER)
- UNIQUE(user_id, name)
```

## Project Structure

```
NoteBook/
├── NoteBook.Domain/              # Domain layer (entities, interfaces)
├── NoteBook.Application/         # Application layer (commands, queries, handlers)
├── NoteBook.Infrastructure/      # Infrastructure layer (repositories, data access)
├── NoteBook.Web/                 # Web layer (controllers, views)
├── db/                           # Database files
│   ├── schema.sql               # Database schema initialization
│   ├── migrations/              # Schema migrations
│   └── seeds/                   # Sample data
├── Dockerfile                    # Docker image definition
├── docker-compose.yml            # Docker Compose configuration
├── .dockerignore                 # Docker build exclusions
└── README.md                     # This file
```

## Development Guidelines

### Adding a New Feature

1. **Define the domain entity** in `NoteBook.Domain/Entities/`
2. **Create the repository interface** in `NoteBook.Domain/Repositories/`
3. **Implement the repository** in `NoteBook.Infrastructure/Repositories/`
4. **Create DTOs** in `NoteBook.Application/DTOs/`
5. **Add MediatR Commands/Queries** in `NoteBook.Application/Features/`
6. **Create handlers** with business logic
7. **Add validation** using FluentValidation
8. **Update the mapping profile** in `NoteBook.Application/Mapping/`
9. **Create API endpoints** in `NoteBook.Web/Controllers/`

### Logging

Logging is configured using Serilog. Logs are written to:
- Console (development)
- File: `logs/app-{date}.txt` (daily rolling)

To enable debug logging, set environment variable:
```bash
ASPNETCORE_ENVIRONMENT=Development
```

### Database Migrations

Currently, the database schema is initialized via `db/schema.sql`. For future changes:

1. Update the SQL script
2. Run migrations manually or create a migration runner
3. Test thoroughly before deploying to production

## Configuration

### appsettings.json

Key configuration options:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=notebook_db;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to "Development" or "Production"
- `ConnectionStrings__DefaultConnection` - Database connection string
- `ASPNETCORE_URLS` - Server URLs (e.g., "http://+:5000")

## Security Considerations

⚠️ **Important**: The current implementation includes placeholder user IDs. Before deploying to production:

1. **Implement Authentication**
   - Integrate ASP.NET Core Identity or JWT
   - Extract actual user ID from claims
   
2. **Add Authorization**
   - Enforce resource-level authorization
   - Prevent users from accessing other users' notes

3. **Input Validation**
   - Add comprehensive FluentValidation rules
   - Sanitize user inputs

4. **Environment Configuration**
   - Use User Secrets during development
   - Use environment variables or Azure Key Vault in production
   - Never commit sensitive information

## Performance Optimization

- PostgreSQL indexes are created for:
  - `notes.user_id` - User-based queries
  - `notes.user_id, is_archived` - Active notes filtering
  - Full-text search on title + content
  - `tags.user_id` - Tag lookups

- Implement pagination for large result sets
- Consider Redis caching for frequently accessed data
- Use database connection pooling (Npgsql handles this)

## Testing

To add tests, create a new project:

```bash
dotnet new xunit -n NoteBook.Tests
dotnet add NoteBook.Tests/NoteBook.Tests.csproj package Moq
dotnet add NoteBook.Tests/NoteBook.Tests.csproj package Testcontainers
```

## Deployment

### Docker

```bash
# Build and push to registry
docker build -t your-registry/notebook:latest .
docker push your-registry/notebook:latest

# Deploy
docker-compose -f docker-compose.yml up -d
```

### On-Premises / VM

1. Install .NET 9 runtime
2. Install PostgreSQL
3. Clone repository
4. Update configuration files
5. Run the application

## Troubleshooting

### Database Connection Issues

```bash
# Test PostgreSQL connection
psql -h localhost -U postgres -d notebook_db

# Check connection string in appsettings.json
```

### Build Failures

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Port Already in Use

- Change the port in `appsettings.json`
- Or set `ASPNETCORE_URLS` environment variable

## Contributing

1. Create a feature branch
2. Make your changes
3. Ensure the solution builds
4. Commit with clear messages
5. Submit a pull request

## License

MIT License - see LICENSE file for details

## Support

For issues and questions:
1. Check the GitHub Issues
2. Review the documentation
3. Create a new issue with details

---

**Version**: 1.0.0  
**Last Updated**: June 2026  
**Maintainer**: NoteBook Development Team
