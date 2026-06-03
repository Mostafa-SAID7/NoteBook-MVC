namespace NoteBook.Domain.Entities;

using NoteBook.Domain.Enums;
using NoteBook.Domain.ValueObjects;

/// <summary>
/// Represents a note entity in the system.
/// Supports rich text content, tagging, soft deletes, and multiple statuses.
/// </summary>
public class Note : BaseEntity
{
    /// <summary>
    /// Note title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// URL-friendly slug for the note (unique per user)
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Rich text content
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Comma-separated tags for categorization and search
    /// </summary>
    public string Tags { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner of the note
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Current status of the note
    /// </summary>
    public NoteStatus Status { get; set; } = NoteStatus.Published;
    
    /// <summary>
    /// Who can access this note
    /// </summary>
    public NoteVisibility Visibility { get; set; } = NoteVisibility.Private;
    
    /// <summary>
    /// Archived notes are hidden by default but not deleted
    /// </summary>
    public bool IsArchived { get; set; }
    
    /// <summary>
    /// Timestamp when note was archived (UTC)
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    
    /// <summary>
    /// User rating (0-5 stars, optional)
    /// </summary>
    public int? Rating { get; set; }
    
    /// <summary>
    /// Color coding for notes (optional)
    /// </summary>
    public string? ColorHex { get; set; }
    
    /// <summary>
    /// Number of times this note was viewed
    /// </summary>
    public int ViewCount { get; set; }
    
    /// <summary>
    /// Verify that required fields are populated
    /// </summary>
    public bool IsValid() => 
        !string.IsNullOrWhiteSpace(Title) && 
        !string.IsNullOrWhiteSpace(Content) && 
        !string.IsNullOrWhiteSpace(Slug) &&
        UserId != Guid.Empty;
    
    /// <summary>
    /// Archive this note
    /// </summary>
    public void Archive(Guid archivedBy)
    {
        IsArchived = true;
        ArchivedAt = DateTime.UtcNow;
        Status = NoteStatus.Archived;
        UpdateAudit(archivedBy);
    }
    
    /// <summary>
    /// Unarchive this note
    /// </summary>
    public void Unarchive(Guid unarchivedBy)
    {
        IsArchived = false;
        ArchivedAt = null;
        Status = NoteStatus.Published;
        UpdateAudit(unarchivedBy);
    }
    
    /// <summary>
    /// Increment view count
    /// </summary>
    public void IncrementViewCount() => ViewCount++;
}
