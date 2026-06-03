namespace NoteBook.Domain.Enums;

/// <summary>
/// Note visibility enumeration
/// Controls who can access a note
/// </summary>
public enum NoteVisibility
{
    /// <summary>
    /// Only the owner can view
    /// </summary>
    Private = 0,
    
    /// <summary>
    /// Specific users can view (requires sharing setup)
    /// </summary>
    Shared = 1,
    
    /// <summary>
    /// Anyone with link can view
    /// </summary>
    LinkOnly = 2,
    
    /// <summary>
    /// Public on web
    /// </summary>
    Public = 3
}
