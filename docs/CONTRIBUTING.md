# Contributing to NoteBook

Thank you for your interest in contributing to NoteBook! This document provides guidelines for contributing.

## Code of Conduct

- Be respectful and inclusive
- No harassment or discrimination
- Focus on constructive feedback
- Respect others' time and effort

## Getting Started

1. **Fork the repository**
   ```bash
   Click "Fork" button on GitHub
   ```

2. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR-USERNAME/NoteBook.git
   cd NoteBook
   ```

3. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/ORIGINAL-REPO/NoteBook.git
   ```

4. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

See [SETUP.md](../SETUP.md) for detailed local development setup.

## Making Changes

### Code Style

- **Language**: C# (C# 12 features supported)
- **Conventions**: Follow Microsoft C# coding conventions
- **Formatting**: Use `.editorconfig` settings
- **Comments**: Document complex logic

### Coding Standards

```csharp
// ✅ Good
public async Task<Note?> GetNoteByIdAsync(Guid id, CancellationToken cancellationToken)
{
    var note = await _repository.GetByIdAsync(id, cancellationToken);
    return _mapper.Map<Note>(note);
}

// ❌ Bad
public Note GetNoteById(Guid id)
{
    return _repository.GetNoteById(id);
}
```

**Guidelines**:
- Use `async/await` for I/O operations
- Include `CancellationToken` parameter
- Use `nullable` reference types (`?`)
- Add XML documentation comments on public members
- Keep methods focused and small

### Naming Conventions

```csharp
public class MyFeatureCommand { }           // Commands: {Feature}Command
public class MyFeatureCommandHandler { }    // Handlers: {Feature}Handler
public class MyFeatureQuery { }             // Queries: {Feature}Query
public class MyFeatureRepository { }        // Repositories: {Feature}Repository
public class MyFeatureDto { }               // DTOs: {Feature}Dto
```

## Adding Features

### Step 1: Create Feature Branch
```bash
git checkout -b feature/user-authentication
```

### Step 2: Plan Your Changes

Before coding, consider:
- Does it follow clean architecture?
- What layer(s) does it affect?
- Are there database schema changes?
- What tests are needed?

### Step 3: Implement

1. **Add Domain Layer**
   - Entities in `NoteBook.Domain/Entities/`
   - Interfaces in `NoteBook.Domain/Repositories/`

2. **Add Infrastructure Layer**
   - Repository implementations in `NoteBook.Infrastructure/Repositories/`
   - Database initialization scripts

3. **Add Application Layer**
   - Commands/Queries in `NoteBook.Application/Features/`
   - DTOs in `NoteBook.Application/DTOs/`
   - Handlers with business logic

4. **Add Web Layer**
   - Controllers in `NoteBook.Web/Controllers/`
   - Views if needed in `NoteBook.Web/Views/`

### Step 4: Test

```bash
# Build
dotnet build

# Run tests (if any)
dotnet test

# Manual testing
dotnet run
```

### Step 5: Documentation

- Update relevant `.md` files
- Add inline code comments for complex logic
- Update API documentation if endpoints changed

## Commit Messages

Follow conventional commits format:

```
type(scope): subject

body (optional)

footer (optional)
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style (formatting, missing semicolons, etc.)
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Test additions or updates
- `chore`: Build/tooling/dependencies

**Examples**:
```bash
git commit -m "feat(auth): implement JWT authentication"
git commit -m "fix(notes): prevent user access to others' notes"
git commit -m "docs: update API documentation"
git commit -m "refactor(repository): optimize database queries"
```

## Pull Requests

### Before Submitting

1. **Update from main**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Build and test locally**
   ```bash
   dotnet clean
   dotnet build
   dotnet test
   ```

3. **Review your own code**
   - Check for typos
   - Remove debug code
   - Verify error handling

### Creating PR

1. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Go to GitHub and create PR**
   - Title: Clear, concise description
   - Description: Use template below
   - Reference: Link related issues

### PR Description Template

```markdown
## Description
Brief explanation of what this PR does.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issue
Fixes #(issue number)

## Changes Made
- Change 1
- Change 2
- Change 3

## Testing
- [ ] Unit tests added
- [ ] Integration tests added
- [ ] Tested locally on Windows/Mac/Linux
- [ ] Docker tested

## Checklist
- [ ] Code follows style guidelines
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No breaking changes
- [ ] All tests pass
```

## Code Review Process

### For PR Authors
- Be responsive to feedback
- Update PR based on comments
- Request re-review after changes
- Keep discussion professional and constructive

### For Reviewers
- Be respectful and constructive
- Suggest improvements, don't demand
- Consider context and tradeoffs
- Approve when satisfied

## Testing Guidelines

### Unit Tests
```csharp
[Fact]
public async Task CreateNoteCommand_WithValidInput_CreatesAndReturnsNote()
{
    // Arrange
    var command = new CreateNoteCommand(
        UserId: Guid.NewGuid(),
        Title: "Test",
        Content: "Test content",
        Tags: ""
    );
    var mockRepository = new Mock<INoteRepository>();
    var mockMapper = new Mock<IMapper>();
    var handler = new CreateNoteCommandHandler(mockRepository.Object, mockMapper.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    mockRepository.Verify(x => x.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Tests
```csharp
[Fact]
public async Task CreateNote_EndToEnd_SavesAndRetrievesFromDatabase()
{
    // Use Testcontainers for real PostgreSQL
    // Test full flow from API to database
}
```

## Documentation

### Updating README.md
- Keep it concise
- Update if architecture changes
- Add examples for new features

### Adding Code Comments
```csharp
// ✅ Good: Explains WHY
// We need to check archived status before deleting to prevent 
// users from accidentally deleting archived notes
var isArchived = note.IsArchived;

// ❌ Bad: Explains WHAT (obvious from code)
// Check if note is archived
var isArchived = note.IsArchived;
```

### Updating API Documentation
- Add endpoint descriptions in `docs/API.md`
- Include request/response examples
- Document error codes

## Database Changes

### Schema Changes

1. **Update `database.sql`**
   ```sql
   ALTER TABLE notes ADD COLUMN new_column VARCHAR(255);
   ```

2. **Create migration script**
   ```
   migrations/001_add_new_column.sql
   ```

3. **Document in PR**
   - Explain why change is needed
   - Consider data migration for existing databases

### Testing Schema Changes

```bash
# Drop database
psql -U postgres -c "DROP DATABASE notebook_db;"

# Recreate
psql -U postgres -c "CREATE DATABASE notebook_db;"

# Initialize
psql -U postgres -d notebook_db -f database.sql

# Verify changes
psql -U postgres -d notebook_db -c "\d notes"
```

## Performance Considerations

- Use `async/await` for I/O operations
- Leverage database indexes
- Consider pagination for large datasets
- Profile before optimizing

## Security Considerations

- Never commit secrets (API keys, passwords)
- Validate all user inputs
- Use parameterized queries (Dapper does this)
- Implement proper authorization checks
- Review OWASP Top 10

## Reporting Issues

### Bug Reports

Include:
1. Description of the bug
2. Steps to reproduce
3. Expected behavior
4. Actual behavior
5. Environment (OS, .NET version)
6. Logs/error messages

**Template**:
```markdown
## Description
What is the bug?

## Steps to Reproduce
1. ...
2. ...

## Expected Behavior
What should happen?

## Actual Behavior
What actually happens?

## Environment
- OS: Windows 10
- .NET: 9.0
- PostgreSQL: 16

## Logs
```
error logs here
```
```

### Feature Requests

Include:
1. Clear description of feature
2. Use case / why it's needed
3. Proposed implementation (optional)
4. Alternative solutions considered

## Resources

- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft REST API Guidelines](https://github.com/Microsoft/api-guidelines)
- [Clean Code by Robert C. Martin](https://www.oreilly.com/library/view/clean-code-a/9780136083238/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)

## Questions?

- Open a GitHub Discussion
- Check existing issues
- Read documentation

---

**Thank you for contributing!**

We appreciate every contribution, big or small. Your help makes NoteBook better! 🎉
