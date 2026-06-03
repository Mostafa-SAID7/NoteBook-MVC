namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents a tag for categorizing and searching notes.
/// Tags can be reused across multiple notes.
/// </summary>
public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int UsageCount { get; set; }
}
