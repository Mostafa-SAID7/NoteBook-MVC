# Development Guide

Guide for developers working on NoteBook codebase.

## Getting Started

1. Follow [SETUP.md](../SETUP.md) to set up local development environment
2. Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand code structure
3. Review [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards

## Project Organization

```
NoteBook/
├── NoteBook.Domain/              # Core business domain
├── NoteBook.Application/         # Application services & CQRS
├── NoteBook.Infrastructure/      # Data access & external services
├── NoteBook.Web/                 # API & presentation
├── docs/                         # Documentation
├── database.sql                  # Database schema
├── Dockerfile                    # Docker image
├── docker-compose.yml            # Local environment
└── README.md                     # Project overview
```

## Development Workflow

### 1. Create Feature Branch

```bash
# Update main branch
git checkout main
git pull origin main

# Create feature branch
git checkout -b feature/my-feature
```

### 2. Implement Feature

Follow the clean architecture pattern:

**Step 1: Domain Layer (NoteBook.Domain)**

```csharp
// Entities/MyEntity.cs
public class MyEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    // ... properties
}

// Repositories/IMyRepository.cs
public interface IMyRepository : IRepository<MyEntity, Guid>
{
    Task<MyEntity?> GetByNameAsync(string name, CancellationToken cancellationToken);
}
```

**Step 2: Infrastructure Layer (NoteBook.Infrastructure)**

```csharp
// Repositories/MyRepository.cs
public class MyRepository : IMyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public MyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<MyEntity?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.GetConnection();
        const string sql = "SELECT * FROM my_entities WHERE name = @Name";
        return await connection.QuerySingleOrDefaultAsync<MyEntity>(sql, new { Name = name });
    }
}
```

**Step 3: Application Layer (NoteBook.Application)**

```csharp
// Features/MyFeatures/Commands/CreateMyEntityCommand.cs
public record CreateMyEntityCommand(string Name) : IRequest<MyEntityDto>;

// Features/MyFeatures/Commands/CreateMyEntityCommandHandler.cs
public class CreateMyEntityCommandHandler : IRequestHandler<CreateMyEntityCommand, MyEntityDto>
{
    private readonly IMyRepository _repository;
    private readonly IMapper _mapper;
    
    public async Task<MyEntityDto> Handle(CreateMyEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = new MyEntity { Name = request.Name };
        var created = await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<MyEntityDto>(created);
    }
}

// DTOs/MyEntityDto.cs
public class MyEntityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// Mapping/MappingProfile.cs (update)
CreateMap<MyEntity, MyEntityDto>().ReverseMap();
```

**Step 4: Web Layer (NoteBook.Web)**

```csharp
// Controllers/MyEntitiesController.cs
[ApiController]
[Route("api/[controller]")]
public class MyEntitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    public async Task<ActionResult<MyEntityDto>> Create([FromBody] CreateMyEntityRequest request)
    {
        var command = new CreateMyEntityCommand(request.Name);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), result);
    }
}
```

### 3. Database Changes

If your feature requires database schema changes:

```sql
-- database.sql (add at end)
CREATE TABLE my_entities (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_my_entities_name ON my_entities(name);
```

Then:
```bash
# Drop and recreate local database
psql -U postgres -c "DROP DATABASE notebook_db;"
psql -U postgres -c "CREATE DATABASE notebook_db;"
psql -U postgres -d notebook_db -f database.sql
```

### 4. Testing

```bash
# Build
dotnet build

# Run application
cd NoteBook.Web
dotnet run

# Test in another terminal
curl -X GET http://localhost:5000/api/my-entities
```

### 5. Commit Changes

```bash
# Stage changes
git add .

# Commit with conventional commit format
git commit -m "feat(entities): add my entity feature"

# Push to fork
git push origin feature/my-feature
```

## Debugging

### Visual Studio

1. Open `NoteBook.sln`
2. Set breakpoint by clicking line number
3. Press F5 to start debugging
4. Application pauses at breakpoint

### VS Code

1. Open NoteBook folder
2. Press F5 to launch debugger
3. Set breakpoints as needed

### Debug Console Logging

```csharp
// Quick debug output
System.Diagnostics.Debug.WriteLine($"Value: {myVar}");
```

## Code Standards

### Naming Conventions

```csharp
// Classes
public class CreateNoteCommand { }       // PascalCase

// Methods & Properties
public async Task<Note> GetNoteAsync() { }  // PascalCase

// Parameters & Variables
var userId = Guid.NewGuid();            // camelCase

// Constants
private const string TableName = "notes";   // PascalCase

// Interfaces
public interface IRepository { }        // IPascalCase
```

### Async/Await Pattern

```csharp
// ✅ Good: Always async for I/O operations
public async Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
{
    using var connection = _connectionFactory.GetConnection();
    return await connection.QuerySingleOrDefaultAsync<Note>(sql);
}

// ❌ Bad: Sync blocking call
public Note? GetById(Guid id)
{
    using var connection = _connectionFactory.GetConnection();
    return connection.QuerySingleOrDefault<Note>(sql);
}
```

### Null Safety

```csharp
// ✅ Good: Explicit null checks and nullable reference types
public class Note
{
    public string? Description { get; set; }  // Can be null
    public string Title { get; set; } = string.Empty;  // Cannot be null
}

// ❌ Bad: No indication of nullability
public class Note
{
    public string Description { get; set; }
    public string Title { get; set; }
}
```

### Error Handling

```csharp
// ✅ Good: Specific exceptions and logging
try
{
    await _repository.AddAsync(note, cancellationToken);
}
catch (DbException ex)
{
    _logger.LogError(ex, "Database error adding note");
    throw new DomainException("Failed to create note", ex);
}

// ❌ Bad: Generic catch-all
try
{
    await _repository.AddAsync(note, cancellationToken);
}
catch (Exception)
{
    // Silently fail
}
```

## Performance Optimization

### Database Queries

```csharp
// ✅ Good: Use indexes, pagination, specific columns
const string sql = @"
    SELECT id, title, content FROM notes
    WHERE user_id = @UserId AND is_deleted = false
    ORDER BY updated_at DESC
    LIMIT @PageSize OFFSET @Offset";

// ❌ Bad: SELECT *, no pagination, missing index
const string sql = "SELECT * FROM notes ORDER BY updated_at DESC";
```

### Connection Management

```csharp
// ✅ Good: Use connection factory and dispose
using var connection = _connectionFactory.GetConnection();
var result = await connection.QueryAsync<Note>(sql);

// ❌ Bad: Create new connection
var connection = new NpgsqlConnection(connectionString);
var result = connection.Query<Note>(sql);
connection.Close();
```

### Caching Strategy

```csharp
// Future enhancement: Cache frequently accessed data
public async Task<IEnumerable<Tag>> GetUserTagsAsync(Guid userId, CancellationToken cancellationToken)
{
    var cacheKey = $"user_tags_{userId}";
    if (_cache.TryGetValue(cacheKey, out var cached))
        return (IEnumerable<Tag>)cached;
    
    var tags = await _repository.GetByUserIdAsync(userId, cancellationToken);
    _cache.Set(cacheKey, tags, TimeSpan.FromHours(1));
    
    return tags;
}
```

## Useful Commands

### Build & Run

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Build specific project
dotnet build NoteBook.Web

# Run application
cd NoteBook.Web && dotnet run

# Run with specific environment
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### Database

```bash
# Connect to database
psql -U postgres -d notebook_db

# List tables
psql -U postgres -d notebook_db -c "\dt"

# Describe table
psql -U postgres -d notebook_db -c "\d notes"

# Run query
psql -U postgres -d notebook_db -c "SELECT COUNT(*) FROM notes;"
```

### Git

```bash
# View branch status
git status

# View changes
git diff

# View commit history
git log --oneline

# Stage specific file
git add NoteBook.Web/Controllers/NotesController.cs

# Unstage changes
git reset

# Discard changes
git checkout -- .
```

### Docker

```bash
# Build image
docker build -t notebook:latest .

# Run container
docker run -p 5000:5000 notebook:latest

# View logs
docker logs -f <container_id>

# Execute command in container
docker exec <container_id> curl http://localhost:5000/health
```

## Useful Extensions (VS Code)

- **C# Dev Kit** - Core C# support
- **REST Client** - Test API endpoints
- **Docker** - Docker integration
- **Git Graph** - Visual git history
- **Prettier** - Code formatter
- **SonarLint** - Code quality

## Common Tasks

### Add New Validation Rule

```csharp
// Application/Features/Notes/Commands/CreateNoteCommandValidator.cs
public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");
    }
}
```

### Add New Repository Method

```csharp
// Domain/Repositories/INoteRepository.cs
Task<IEnumerable<Note>> GetByDateRangeAsync(
    Guid userId,
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken);

// Infrastructure/Repositories/NoteRepository.cs
public async Task<IEnumerable<Note>> GetByDateRangeAsync(
    Guid userId,
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken)
{
    using var connection = _connectionFactory.GetConnection();
    const string sql = @"
        SELECT * FROM notes
        WHERE user_id = @UserId 
        AND created_at BETWEEN @StartDate AND @EndDate
        AND is_deleted = false";
    
    return await connection.QueryAsync<Note>(sql, 
        new { UserId = userId, StartDate = startDate, EndDate = endDate });
}
```

### Add New API Endpoint

```csharp
// NoteBook.Web/Controllers/NotesController.cs
[HttpGet("by-date")]
public async Task<ActionResult<IEnumerable<NoteDto>>> GetByDateRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    CancellationToken cancellationToken)
{
    var query = new GetNotesByDateRangeQuery(DefaultUserId, startDate, endDate);
    var result = await _mediator.Send(query, cancellationToken);
    return Ok(result);
}
```

## Troubleshooting Development

### Hot Reload Not Working

```bash
# Restart application
# The application watches for changes and recompiles on save
# If not working, manually restart: Ctrl+C, then dotnet run
```

### Breakpoints Not Hitting

1. Ensure running in Debug mode (not Release)
2. Rebuild solution
3. Clear bin/obj folders: `dotnet clean`
4. Restart debugger

### Changes Not Appearing

```bash
# Clean build
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

## Resources

- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [MediatR Documentation](https://jbogard.com/mediatr/)
- [Dapper GitHub](https://github.com/DapperLib/Dapper)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

**Version**: 1.0.0  
**Last Updated**: June 2026
