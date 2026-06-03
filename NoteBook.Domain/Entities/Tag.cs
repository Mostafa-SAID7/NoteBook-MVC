namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents a tag for categorizing and searching notes.
/// Tags can be reused across multiple notes and users.
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// Tag name (unique per user)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// URL-friendly slug for the tag
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner of this tag
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// How many times this tag is used
    /// </summary>
    public int UsageCount { get; set; }
    
    /// <summary>
    /// Tag description (optional)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Color for tag display (optional, hex format)
    /// </summary>
    public string? ColorHex { get; set; }
    
    /// <summary>
    /// Tag icon/emoji (optional)
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Increment usage count when tag is used
    /// </summary>
    public void IncrementUsage()
    {
        UsageCount++;
    }
    
    /// <summary>
    /// Decrement usage count when tag is removed from a note
    /// </summary>
    public void DecrementUsage()
    {
        if (UsageCount > 0)
            UsageCount--;
    }
}
