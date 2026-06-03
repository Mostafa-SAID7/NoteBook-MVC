namespace NoteBook.Domain.Enums;

/// <summary>
/// User role enumeration
/// Represents different permission levels in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular user - standard access
    /// </summary>
    User = 0,
    
    /// <summary>
    /// Moderator - can manage other users' content
    /// </summary>
    Moderator = 1,
    
    /// <summary>
    /// Administrator - full system access
    /// </summary>
    Administrator = 2,
    
    /// <summary>
    /// Super administrator - system owner
    /// </summary>
    SuperAdmin = 3
}
