namespace NoteBook.Domain.Entities;

/// <summary>
/// Base entity class providing common properties for all domain entities.
/// Includes soft delete, audit trails, and timestamping.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key - unique identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of user who created this entity
    /// </summary>
    public Guid CreatedBy { get; set; }
    
    /// <summary>
    /// Last modification timestamp (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of user who last modified this entity
    /// </summary>
    public Guid UpdatedBy { get; set; }
    
    /// <summary>
    /// Soft delete flag - entity is logically deleted but not removed from database
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Timestamp when entity was deleted (UTC)
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// ID of user who deleted this entity
    /// </summary>
    public Guid? DeletedBy { get; set; }
    
    /// <summary>
    /// Mark entity as deleted (soft delete)
    /// </summary>
    public void SoftDelete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    
    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
    
    /// <summary>
    /// Update modification audit information
    /// </summary>
    public void UpdateAudit(Guid updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
