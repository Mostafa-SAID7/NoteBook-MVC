namespace NoteBook.Domain.Exceptions;

/// <summary>
/// Thrown when a note cannot be found
/// </summary>
public class NoteNotFoundException : DomainException
{
    public Guid NoteId { get; }
    
    public NoteNotFoundException(Guid noteId) 
        : base($"Note with ID '{noteId}' was not found.")
    {
        NoteId = noteId;
    }
}
