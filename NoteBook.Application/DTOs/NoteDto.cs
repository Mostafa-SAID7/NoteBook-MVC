namespace NoteBook.Application.DTOs;

/// <summary>
/// Data Transfer Object for Note
/// </summary>
public class NoteDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string Tags { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsArchived { get; set; }
    
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Create/Update note request DTO
/// </summary>
public class CreateOrUpdateNoteRequest
{
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string Tags { get; set; } = string.Empty;
}
