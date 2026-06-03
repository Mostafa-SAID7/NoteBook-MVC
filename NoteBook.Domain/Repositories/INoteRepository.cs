namespace NoteBook.Domain.Repositories;

using NoteBook.Domain.Entities;

/// <summary>
/// Repository interface for Note operations
/// </summary>
public interface INoteRepository : IRepository<Note, Guid>
{
    /// <summary>
    /// Get all active notes for a specific user
    /// </summary>
    Task<IEnumerable<Note>> GetUserNotesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get archived notes for a specific user
    /// </summary>
    Task<IEnumerable<Note>> GetUserArchivedNotesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search notes by title and content for a specific user
    /// </summary>
    Task<IEnumerable<Note>> SearchNotesAsync(Guid userId, string searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get notes by tag for a specific user
    /// </summary>
    Task<IEnumerable<Note>> GetNotesByTagAsync(Guid userId, string tag, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated notes for a user
    /// </summary>
    Task<(IEnumerable<Note> Items, int Total)> GetUserNotesPagedAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Soft delete a note
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid noteId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Restore a soft-deleted note
    /// </summary>
    Task<bool> RestoreAsync(Guid noteId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Archive a note
    /// </summary>
    Task<bool> ArchiveAsync(Guid noteId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unarchive a note
    /// </summary>
    Task<bool> UnarchiveAsync(Guid noteId, CancellationToken cancellationToken = default);
}
