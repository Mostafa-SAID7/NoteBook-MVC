namespace NoteBook.Application.Features.Notes.Commands;

using MediatR;
using NoteBook.Domain.Exceptions;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for deleting (soft delete) a note
/// </summary>
public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, bool>
{
    private readonly INoteRepository _noteRepository;
    
    public DeleteNoteCommandHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    
    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        
        if (note is null)
            throw new NoteNotFoundException(request.NoteId);
        
        // Verify ownership
        if (note.UserId != request.UserId)
            throw new DomainException("User does not have permission to delete this note.");
        
        var result = await _noteRepository.SoftDeleteAsync(request.NoteId, cancellationToken);
        await _noteRepository.SaveChangesAsync(cancellationToken);
        
        return result;
    }
}
