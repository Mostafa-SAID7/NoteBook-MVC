# Domain Layer Architecture

Complete guide to the NoteBook Domain layer - the heart of the business logic and data model.

---

## Overview

The Domain layer contains:
- **Entities** - Core business objects
- **Value Objects** - Immutable data types
- **Enums** - Type-safe constants
- **Exceptions** - Domain-specific errors
- **Repositories** - Data access interfaces

---

## Layer Structure

```
NoteBook.Domain/
├── Entities/
│   ├── BaseEntity.cs          # Abstract base for all entities
│   ├── Note.cs                # Note entity
│   ├── ApplicationUser.cs      # User entity
│   ├── Tag.cs                 # Tag entity
│   ├── NoteShare.cs           # Sharing entity (future)
│   ├── NoteComment.cs         # Comments entity (future)
│   ├── NoteAttachment.cs      # Attachments entity (future)
│   └── NoteActivity.cs        # Activity log entity (future)
├── Enums/
│   ├── NoteStatus.cs          # Note lifecycle states
│   ├── NoteVisibility.cs      # Access control levels
│   └── UserRole.cs            # User permission levels
├── ValueObjects/
│   ├── Result.cs              # Result<T> for functional error handling
│   ├── Slug.cs                # SEO-friendly slugs
│   └── Email.cs               # Email validation
├── Exceptions/
│   ├── DomainException.cs     # Base exception
│   └── NoteNotFoundException.cs
└── Repositories/
    ├── IRepository.cs         # Generic repository interface
    └── INoteRepository.cs     # Note-specific repository
```

---

## Core Concepts

### 1. BaseEntity - Foundation for All Entities

All domain entities inherit from `BaseEntity`, providing consistent:
- **Unique Identity** - GUID primary key
- **Audit Trail** - CreatedBy, UpdatedBy tracking
- **Soft Delete** - Logical deletion with restoration
- **Timestamps** - CreatedAt, UpdatedAt (UTC)

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Methods for business operations
    public void SoftDelete(Guid deletedBy) { ... }
    public void Restore() { ... }
    public void UpdateAudit(Guid updatedBy) { ... }
}
```

**Benefits:**
- ✅ Consistent entity behavior across domain
- ✅ Built-in audit logging for compliance
- ✅ Soft delete prevents accidental data loss
- ✅ Track who made changes when

---

### 2. Entities

#### Note Entity

Represents a user's note with rich features:

```csharp
public class Note : BaseEntity
{
    public string Title { get; set; }        // Required, max 255
    public string Slug { get; set; }         // URL-friendly (unique per user)
    public string Content { get; set; }      // Rich text
    public string Tags { get; set; }         // Comma-separated
    
    public Guid UserId { get; set; }         // Owner
    
    public NoteStatus Status { get; set; }   // Draft/Published/Archived/Deleted
    public NoteVisibility Visibility { get; set; } // Private/Shared/LinkOnly/Public
    
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    
    public int? Rating { get; set; }         // 0-5 stars
    public string? ColorHex { get; set; }    // Color coding
    public int ViewCount { get; set; }       // Analytics
    
    // Business methods
    public void Archive(Guid archivedBy) { ... }
    public void Unarchive(Guid unarchivedBy) { ... }
    public void IncrementViewCount() { ... }
}
```

**Usage:**
```csharp
// Create new note
var note = new Note 
{ 
    Title = "My First Note",
    Slug = "my-first-note",
    Content = "Rich content here",
    UserId = currentUserId,
    CreatedBy = currentUserId
};

// Archive it
note.Archive(currentUserId);

// Restore if needed
note.Unarchive(currentUserId);

// Soft delete (permanent deletion)
note.SoftDelete(currentUserId);
```

#### ApplicationUser Entity

Represents a system user with roles and preferences:

```csharp
public class ApplicationUser : BaseEntity
{
    public string Email { get; set; }            // Unique
    public string UserName { get; set; }         // Unique
    public string FullName { get; set; }
    public string PasswordHash { get; set; }
    
    public UserRole Role { get; set; }           // User/Moderator/Admin/SuperAdmin
    
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public string? PreferredTheme { get; set; }  // light/dark
    public string PreferredLanguage { get; set; } // en, fr, etc
    
    public int TotalNotesCreated { get; set; }
    public string? Bio { get; set; }
    
    // Business methods
    public void Deactivate(Guid deactivatedBy) { ... }
    public void Activate(Guid activatedBy) { ... }
    public void UpdateLastLogin() { ... }
    public void IncrementNoteCount() { ... }
}
```

#### Tag Entity

Manages note tags with usage tracking:

```csharp
public class Tag : BaseEntity
{
    public string Name { get; set; }          // Unique per user
    public string Slug { get; set; }          // URL-friendly
    public Guid UserId { get; set; }          // Owner
    
    public int UsageCount { get; set; }       // How many notes use this
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public string? Icon { get; set; }          // Emoji or icon
    
    // Business methods
    public void IncrementUsage() { ... }
    public void DecrementUsage() { ... }
}
```

#### Future Entities

Pre-designed for upcoming features:

- **NoteShare** - Sharing with other users
- **NoteComment** - Collaborative discussions
- **NoteAttachment** - File attachments
- **NoteActivity** - Audit logging and analytics

---

### 3. Value Objects

Value Objects are immutable, self-validating types that represent concepts in the domain.

#### Slug - URL-Friendly Text

```csharp
// Create from title
var result = Slug.CreateFromText("My Amazing Note!");
if (result.IsSuccess)
{
    var slug = result.Value; // "my-amazing-note"
}

// Or create directly with validation
var result = Slug.Create("my-note");

// Handle unique slugs
var result = Slug.CreateUnique("my-note", slug => database.NoteExists(slug));

// Use in entity
var note = new Note { Slug = slug.Value };
```

**Features:**
- ✅ Automatic conversion to lowercase
- ✅ Space to hyphen conversion
- ✅ Special character removal
- ✅ Length validation (3-100 chars)
- ✅ Uniqueness support with conflict resolution
- ✅ Immutable and equatable

#### Email - Validated Email Address

```csharp
var result = Email.Create("user@example.com");

if (result.IsSuccess)
{
    var email = result.Value; // Stored lowercase
    email.GetLocalPart(); // "user"
    email.GetDomain();    // "example.com"
}
else
{
    var error = result.Error; // "Invalid email format"
}
```

**Features:**
- ✅ RFC 5321 compliant validation
- ✅ Case-insensitive (stored lowercase)
- ✅ Immutable and equatable
- ✅ Domain extraction helpers

#### Result<T> - Functional Error Handling

```csharp
// Success case
var result = Result<Note>.Success(note);

// Failure case
var result = Result<Note>.Failure("Note not found");

// Check status
if (result.IsSuccess)
{
    var note = result.Value;
}
else
{
    Console.WriteLine(result.Error);
}

// Transform/Map
var slugResult = result.Map(note => note.Slug);

// Chain operations
var result = GetNote(id)
    .Bind(note => ValidateNote(note))
    .Bind(note => SaveNote(note))
    .Tap(note => LogSuccess(note));

// Get value or throw
var note = result.GetValueOrThrow();

// Get value or default
var note = result.GetValueOrDefault();
```

**Benefits:**
- ✅ Explicit error handling (no exceptions for flow)
- ✅ Type-safe results
- ✅ Functional composition support
- ✅ Chainable operations

---

### 4. Enums - Type-Safe Constants

#### NoteStatus

Represents note lifecycle:

```csharp
public enum NoteStatus
{
    Draft = 0,        // Work in progress
    Published = 1,    // Active and visible
    Archived = 2,     // Inactive but kept
    Deleted = 3       // Soft-deleted
}
```

#### NoteVisibility

Controls access level:

```csharp
public enum NoteVisibility
{
    Private = 0,   // Only owner
    Shared = 1,    // Specific users
    LinkOnly = 2,  // Anyone with link
    Public = 3     // Public web
}
```

#### UserRole

Authorization levels:

```csharp
public enum UserRole
{
    User = 0,         // Regular user
    Moderator = 1,    // Can manage content
    Administrator = 2, // Full system access
    SuperAdmin = 3    // System owner
}
```

---

## Business Logic Examples

### Creating a Note

```csharp
// 1. Create slug
var slugResult = Slug.CreateFromText(noteRequest.Title);
if (!slugResult.IsSuccess)
    throw new DomainException(slugResult.Error);

// 2. Create entity
var note = new Note
{
    Title = noteRequest.Title,
    Slug = slugResult.Value.Value,
    Content = noteRequest.Content,
    Tags = noteRequest.Tags,
    UserId = currentUserId,
    Status = NoteStatus.Published,
    Visibility = NoteVisibility.Private,
    CreatedBy = currentUserId,
    UpdatedBy = currentUserId
};

// 3. Validate
if (!note.IsValid())
    throw new DomainException("Invalid note data");

// 4. Save
await noteRepository.AddAsync(note);

// 5. Update user stats
user.IncrementNoteCount();
```

### Archiving a Note

```csharp
var note = await noteRepository.GetByIdAsync(noteId);

if (note == null)
    throw new NoteNotFoundException(noteId);

if (note.IsDeleted)
    throw new DomainException("Cannot archive deleted note");

note.Archive(currentUserId); // Updates status, timestamp, audit

await noteRepository.UpdateAsync(note);
```

### Soft Deleting (Permanent Deletion)

```csharp
var note = await noteRepository.GetByIdAsync(noteId);

note.SoftDelete(currentUserId); // Marks as deleted, no data loss

await noteRepository.UpdateAsync(note);

// Later: restore if needed
note.Restore();
```

---

## Exception Handling

Domain-specific exceptions provide meaningful error context:

```csharp
try
{
    var note = await noteRepository.GetByIdAsync(noteId);
    
    if (note == null || note.IsDeleted)
        throw new NoteNotFoundException(noteId);
    
    // Business logic
}
catch (NoteNotFoundException ex)
{
    return NotFound(ex.Message);
}
catch (DomainException ex)
{
    return BadRequest(ex.Message);
}
```

---

## Design Patterns Used

### 1. Base Entity Pattern
Provides common behavior for all entities.

### 2. Value Object Pattern
Immutable objects that represent concepts (Email, Slug).

### 3. Result Pattern
Functional error handling without exceptions.

### 4. Repository Pattern
Abstract data access behind interfaces.

### 5. Soft Delete Pattern
Logical deletion instead of hard deletion.

### 6. Audit Trail Pattern
Track who changed what and when.

---

## Adding New Entities

Template for new domain entities:

```csharp
namespace NoteBook.Domain.Entities;

/// <summary>
/// [Entity description]
/// </summary>
public class MyEntity : BaseEntity
{
    // Required properties
    public string Name { get; set; } = string.Empty;
    
    // Related entities
    public Guid UserId { get; set; }
    
    // Optional properties
    public string? Description { get; set; }
    
    // Enums
    public MyStatus Status { get; set; }
    
    // Value Objects
    public string Slug { get; set; } = string.Empty;
    
    // Business methods
    public void DoSomething(Guid userId)
    {
        // Validate preconditions
        if (IsDeleted)
            throw new InvalidOperationException("Cannot operate on deleted entity");
        
        // Perform action
        // ...
        
        // Update audit
        UpdateAudit(userId);
    }
}
```

---

## Testing Domain Entities

Unit test example:

```csharp
[Fact]
public void Archive_ShouldUpdateStatus()
{
    // Arrange
    var userId = Guid.NewGuid();
    var note = new Note 
    { 
        Id = Guid.NewGuid(),
        Title = "Test",
        Content = "Content",
        UserId = userId,
        Status = NoteStatus.Published,
        CreatedBy = userId
    };

    // Act
    note.Archive(userId);

    // Assert
    Assert.True(note.IsArchived);
    Assert.Equal(NoteStatus.Archived, note.Status);
    Assert.NotNull(note.ArchivedAt);
    Assert.Equal(userId, note.UpdatedBy);
}
```

---

## Best Practices

### ✅ DO

- ✅ Use BaseEntity for all entities
- ✅ Use Value Objects for concepts (Email, Slug)
- ✅ Implement business logic in entities
- ✅ Use Enums instead of strings
- ✅ Track audit information
- ✅ Use soft delete for data safety
- ✅ Validate in constructors/methods
- ✅ Keep entities focused and cohesive

### ❌ DON'T

- ❌ Put database logic in entities
- ❌ Use public setters for critical fields
- ❌ Mix concerns (UI, business, data)
- ❌ Create entities without identity
- ❌ Skip validation
- ❌ Hard delete user data
- ❌ Use primitive types for concepts
- ❌ Ignore audit trail

---

## Migration Path

Existing entities have been enhanced with:

1. **BaseEntity inheritance** - Provides audit trails and soft delete
2. **Slugs** - SEO-friendly URLs
3. **Enums** - Status and visibility control
4. **Additional fields** - Ratings, colors, view counts
5. **Business methods** - Archive, restore, increment

Backward compatibility maintained - existing queries still work.

---

## Future Enhancements

Planned additions to Domain layer:

1. **Advanced Specifications** - Complex query filters
2. **Domain Events** - Publish-subscribe pattern
3. **Aggregate Roots** - Transaction boundaries
4. **Custom Exceptions** - More specific error types
5. **Validators** - FluentValidation integration
6. **ReadModels** - Optimized for reads

---

## Related Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) - Overall system design
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development workflow
- [API.md](API.md) - API endpoints using domain layer
- [DEPLOYMENT.md](DEPLOYMENT.md) - Production considerations

---

**Last Updated**: June 2026  
**API Version**: 2.1.0  
**Domain Version**: 2.0.0
