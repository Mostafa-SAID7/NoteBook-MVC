namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents a shared note with another user.
/// Enables collaboration and sharing functionality.
/// </summary>
public class NoteShare : BaseEntity
{
    /// <summary>
    /// The note being shared
    /// </summary>
    public Guid NoteId { get; set; }
    
    /// <summary>
    /// User who owns the note
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// User who the note is shared with
    /// </summary>
    public Guid SharedWithUserId { get; set; }
    
    /// <summary>
    /// Can the shared user edit the note
    /// </summary>
    public bool CanEdit { get; set; }
    
    /// <summary>
    /// Can the shared user delete the note
    /// </summary>
    public bool CanDelete { get; set; }
    
    /// <summary>
    /// Can the shared user share it further
    /// </summary>
    public bool CanShare { get; set; }
    
    /// <summary>
    /// Expiration date for the share (optional)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Is this share still active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Check if share has expired
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    
    /// <summary>
    /// Revoke the share
    /// </summary>
    public void Revoke(Guid revokedBy)
    {
        IsActive = false;
        UpdateAudit(revokedBy);
    }
}
