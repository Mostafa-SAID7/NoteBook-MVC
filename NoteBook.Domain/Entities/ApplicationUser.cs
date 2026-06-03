namespace NoteBook.Domain.Entities;

using NoteBook.Domain.Enums;

/// <summary>
/// Represents an application user.
/// Includes authentication, permissions, and audit information.
/// </summary>
public class ApplicationUser : BaseEntity
{
    /// <summary>
    /// User's email address (unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Username for login (unique)
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's full display name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password (bcrypt or similar)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User role for authorization
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;
    
    /// <summary>
    /// Is user account active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Email verification status
    /// </summary>
    public bool IsEmailVerified { get; set; }
    
    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// User's preferred theme (light/dark)
    /// </summary>
    public string? PreferredTheme { get; set; }
    
    /// <summary>
    /// User's preferred language code (en, fr, etc.)
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";
    
    /// <summary>
    /// Total notes created by user
    /// </summary>
    public int TotalNotesCreated { get; set; }
    
    /// <summary>
    /// User's profile bio/description
    /// </summary>
    public string? Bio { get; set; }
    
    /// <summary>
    /// Deactivate user account
    /// </summary>
    public void Deactivate(Guid deactivatedBy)
    {
        IsActive = false;
        UpdateAudit(deactivatedBy);
    }
    
    /// <summary>
    /// Activate user account
    /// </summary>
    public void Activate(Guid activatedBy)
    {
        IsActive = true;
        UpdateAudit(activatedBy);
    }
    
    /// <summary>
    /// Update last login time
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Increment note counter
    /// </summary>
    public void IncrementNoteCount()
    {
        TotalNotesCreated++;
    }
}
