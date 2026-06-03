namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents an application user.
/// Basic user information for authentication and note ownership.
/// </summary>
public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Email { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLoginAt { get; set; }
}
