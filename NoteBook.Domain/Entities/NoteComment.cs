namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents a comment on a note.
/// Enables collaborative discussion on notes.
/// </summary>
public class NoteComment : BaseEntity
{
    /// <summary>
    /// The note being commented on
    /// </summary>
    public Guid NoteId { get; set; }
    
    /// <summary>
    /// The user who made the comment
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Comment text
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Parent comment ID (for nested comments/replies)
    /// </summary>
    public Guid? ParentCommentId { get; set; }
    
    /// <summary>
    /// Number of likes on this comment
    /// </summary>
    public int LikeCount { get; set; }
    
    /// <summary>
    /// Is this comment marked as resolved
    /// </summary>
    public bool IsResolved { get; set; }
    
    /// <summary>
    /// Edit history timestamp
    /// </summary>
    public DateTime? LastEditedAt { get; set; }
    
    /// <summary>
    /// Mark comment as edited
    /// </summary>
    public void MarkAsEdited(Guid editedBy)
    {
        LastEditedAt = DateTime.UtcNow;
        UpdateAudit(editedBy);
    }
    
    /// <summary>
    /// Resolve a comment (for discussions)
    /// </summary>
    public void Resolve(Guid resolvedBy)
    {
        IsResolved = true;
        UpdateAudit(resolvedBy);
    }
    
    /// <summary>
    /// Increment like count
    /// </summary>
    public void IncrementLikes()
    {
        LikeCount++;
    }
    
    /// <summary>
    /// Decrement like count
    /// </summary>
    public void DecrementLikes()
    {
        if (LikeCount > 0)
            LikeCount--;
    }
}
