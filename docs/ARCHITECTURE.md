# Architecture Overview

## Clean Architecture Pattern

NoteBook follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability.

## Layered Structure

```
┌─────────────────────────────┐
│     Web Layer               │
│  (Controllers, Views, API)  │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│   Application Layer         │
│  (CQRS, DTOs, Validation)   │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│   Domain Layer              │
│  (Entities, Interfaces)     │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│  Infrastructure Layer       │
│  (Repositories, DB Access)  │
└─────────────────────────────┘
```

## Project Structure

### NoteBook.Domain
**Purpose**: Core business logic and domain models

**Contains**:
- **Entities/**: Business entities (Note, ApplicationUser, Tag)
- **Exceptions/**: Domain-specific exceptions
- **Repositories/**: Repository interface contracts

**Key Principle**: No external dependencies. Pure business logic.

### NoteBook.Application
**Purpose**: Application orchestration and CQRS implementation

**Contains**:
- **Features/**: Command and Query handlers organized by feature
  - Commands/: Write operations (Create, Update, Delete)
  - Queries/: Read operations (Get, Search, List)
- **DTOs/**: Data Transfer Objects for API requests/responses
- **Mapping/**: AutoMapper profiles for entity-to-DTO conversion
- **DependencyInjection.cs**: Service registration

**Key Pattern**: CQRS (Command Query Responsibility Segregation)

### NoteBook.Infrastructure
**Purpose**: External concerns and data access implementation

**Contains**:
- **Data/**: Database connection factory
- **Repositories/**: Dapper-based repository implementations
- **DependencyInjection.cs**: Infrastructure service registration

**Tech Stack**: Dapper, Npgsql, PostgreSQL

### NoteBook.Web
**Purpose**: Presentation layer and API endpoints

**Contains**:
- **Controllers/**: API endpoint definitions
- **Models/**: View models for MVC views
- **Views/**: Razor views (if using MVC)
- **Program.cs**: Application startup and configuration

**Tech Stack**: ASP.NET Core 9, MediatR

## CQRS Pattern

Commands and Queries are separated for clear responsibility:

### Commands (Write Operations)
```
User Request → Controller → Command → Handler → Repository → Database
                                         ↓
                                    Update State
```

**Example**: `CreateNoteCommand` → `CreateNoteCommandHandler`

### Queries (Read Operations)
```
User Request → Controller → Query → Handler → Repository → Database
                                      ↓
                                   Return Data
```

**Example**: `GetUserNotesQuery` → `GetUserNotesQueryHandler`

## Data Flow

### Creating a Note

1. **API Request**
   ```
   POST /api/notes
   {
     "title": "My Note",
     "content": "Content here",
     "tags": "tag1,tag2"
   }
   ```

2. **Controller** (Web Layer)
   - Receives `CreateOrUpdateNoteRequest` DTO
   - Creates `CreateNoteCommand`
   - Sends to MediatR

3. **Handler** (Application Layer)
   - `CreateNoteCommandHandler` receives command
   - Validates business rules
   - Calls repository

4. **Repository** (Infrastructure Layer)
   - `NoteRepository.AddAsync()` executes
   - Dapper maps C# object to SQL INSERT
   - Npgsql sends query to PostgreSQL

5. **Response**
   - Returns `NoteDto` to controller
   - Controller returns HTTP 201 with created note

## Dependency Injection

All layers are loosely coupled through dependency injection:

```csharp
// Program.cs
services.AddApplicationServices();      // Application layer
services.AddInfrastructureServices();   // Infrastructure layer
```

**Benefits**:
- Easy to test (mock dependencies)
- Easy to swap implementations
- Follows Dependency Inversion Principle

## Database Access (Dapper)

Why Dapper over Entity Framework?

| Feature | Dapper | EF Core |
|---------|--------|---------|
| Performance | ⚡ Faster | ⚠️ Slower |
| Simplicity | ✅ Lightweight | ❌ Complex |
| Control | ✅ Full SQL control | ⚠️ Limited |
| Learning Curve | ✅ Easy | ❌ Steep |

Dapper provides:
- Raw SQL control
- Lightweight (<100 KB)
- High performance
- Simple mapping

## Validation Strategy

### Fluent Validation
- Input validation in handlers (future enhancement)
- Business rule validation in handlers
- Clear error messages

### Entity Validation
- `Note.IsValid()` method for domain validation

## Error Handling

### Domain Exceptions
- `DomainException`: Base exception for business logic errors
- `NoteNotFoundException`: Specific note not found error

### Global Error Handling
- Middleware catches unhandled exceptions
- Returns standardized error response
- Logs to Serilog

## Logging Strategy

### Serilog Implementation
- **Console**: Development environment
- **File**: `logs/app-{date}.txt` (daily rolling)
- **Level**: Information (production), Debug (development)

**Usage**:
```csharp
_logger.LogInformation("Note created: {NoteId}", noteId);
_logger.LogError(ex, "Error processing note");
```

## Security Architecture

### Current State (Development)
- Hardcoded user ID for testing
- No authentication

### Production Requirements
1. **Authentication**: JWT or ASP.NET Identity
2. **Authorization**: Resource-level access control
3. **Input Validation**: Comprehensive validators
4. **Data Protection**: Encryption for sensitive data
5. **HTTPS**: Enforce in production

## Scalability Considerations

### Horizontal Scaling
- Stateless application design ✅
- Database connection pooling ✅
- Load balancer ready ✅

### Performance Optimization
- Database indexes on `notes.user_id`, `tags.user_id`
- Pagination support
- Async/await throughout
- Connection pooling

### Caching Strategy (Future)
- Redis for frequently accessed tags
- In-memory cache for user preferences
- Cache invalidation on mutations

## Testing Architecture

### Unit Tests (Future)
- Test command handlers in isolation
- Mock repositories
- Assert business logic

### Integration Tests (Future)
- Use Testcontainers for PostgreSQL
- Test repository implementations
- Verify database interactions

### Example Test Structure
```csharp
[Fact]
public async Task CreateNoteCommand_WithValidInput_CreatesNote()
{
    // Arrange
    var command = new CreateNoteCommand(...);
    var handler = new CreateNoteCommandHandler(mockRepo, mockMapper);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(x => x.AddAsync(...));
}
```

## Extension Points

### Adding a New Feature

1. **Define Entity** in `Domain/Entities/`
2. **Define Repository Interface** in `Domain/Repositories/`
3. **Implement Repository** in `Infrastructure/Repositories/`
4. **Create DTOs** in `Application/DTOs/`
5. **Create Commands/Queries** in `Application/Features/`
6. **Create Handlers** with business logic
7. **Add Validators** with FluentValidation
8. **Create API Controller** in `Web/Controllers/`

### Example: Adding Tag Management

```
Domain/Entities/Tag.cs (new)
├── Domain/Repositories/ITagRepository.cs (new)
├── Infrastructure/Repositories/TagRepository.cs (new)
├── Application/DTOs/TagDto.cs (new)
├── Application/Features/Tags/Commands/CreateTagCommand.cs (new)
├── Application/Features/Tags/Queries/GetTagsQuery.cs (new)
└── Web/Controllers/TagsController.cs (new)
```

## Design Patterns Used

| Pattern | Where | Purpose |
|---------|-------|---------|
| CQRS | Application Layer | Separate read and write operations |
| Repository | Infrastructure Layer | Abstract data access |
| Factory | Infrastructure Layer | Create DB connections |
| Dependency Injection | All Layers | Loose coupling |
| Mapper | Application Layer | DTO conversions |
| Middleware | Web Layer | Cross-cutting concerns |

## Conclusion

The architecture provides:
- ✅ Clear separation of concerns
- ✅ Testability
- ✅ Maintainability
- ✅ Scalability
- ✅ Performance
- ✅ Flexibility for future enhancements

---

**Version**: 1.0.0
