namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents a note entity in the system.
/// Supports rich text content, tagging, and soft deletes.
/// </summary>
public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Comma-separated tags for categorization and search
    /// </summary>
    public string Tags { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Soft delete support
    /// </summary>
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Archived notes are hidden by default but not deleted
    /// </summary>
    public bool IsArchived { get; set; }
    
    public DateTime? ArchivedAt { get; set; }
    
    /// <summary>
    /// Verify that required fields are populated
    /// </summary>
    public bool IsValid() => 
        !string.IsNullOrWhiteSpace(Title) && 
        !string.IsNullOrWhiteSpace(Content) && 
        UserId != Guid.Empty;
}
