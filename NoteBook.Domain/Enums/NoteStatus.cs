namespace NoteBook.Domain.Enums;

/// <summary>
/// Note status enumeration
/// Represents different states a note can be in
/// </summary>
public enum NoteStatus
{
    /// <summary>
    /// Draft - not yet finalized
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Published - active and visible
    /// </summary>
    Published = 1,
    
    /// <summary>
    /// Archived - inactive but retained
    /// </summary>
    Archived = 2,
    
    /// <summary>
    /// Deleted - soft-deleted
    /// </summary>
    Deleted = 3
}
