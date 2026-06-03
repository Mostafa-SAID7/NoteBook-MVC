namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents an activity log entry for audit and analytics.
/// Tracks all actions performed on notes.
/// </summary>
public class NoteActivity : BaseEntity
{
    /// <summary>
    /// The note this activity relates to
    /// </summary>
    public Guid NoteId { get; set; }
    
    /// <summary>
    /// User who performed the action
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Type of action (Created, Updated, Deleted, Shared, etc.)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the action
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON snapshot of the entity before the action
    /// </summary>
    public string? BeforeData { get; set; }
    
    /// <summary>
    /// JSON snapshot of the entity after the action
    /// </summary>
    public string? AfterData { get; set; }
    
    /// <summary>
    /// IP address from which the action was performed
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Whether the action was successful
    /// </summary>
    public bool IsSuccessful { get; set; } = true;
    
    /// <summary>
    /// Error message if action failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
    
    /// <summary>
    /// Static method to log activity
    /// </summary>
    public static NoteActivity CreateLog(
        Guid noteId,
        Guid userId,
        string actionType,
        string description,
        bool isSuccessful = true,
        string? errorMessage = null,
        long executionTimeMs = 0)
    {
        return new NoteActivity
        {
            NoteId = noteId,
            UserId = userId,
            ActionType = actionType,
            Description = description,
            IsSuccessful = isSuccessful,
            ErrorMessage = errorMessage,
            ExecutionTimeMs = executionTimeMs,
            CreatedBy = userId,
            UpdatedBy = userId
        };
    }
}
