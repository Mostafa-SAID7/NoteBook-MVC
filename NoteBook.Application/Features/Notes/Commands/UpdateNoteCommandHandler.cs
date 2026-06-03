namespace NoteBook.Application.Features.Notes.Commands;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Exceptions;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for updating an existing note
/// </summary>
public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    public UpdateNoteCommandHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<NoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        
        if (note is null)
            throw new NoteNotFoundException(request.NoteId);
        
        // Verify ownership
        if (note.UserId != request.UserId)
            throw new DomainException("User does not have permission to update this note.");
        
        note.Title = request.Title;
        note.Content = request.Content;
        note.Tags = request.Tags;
        note.UpdatedAt = DateTime.UtcNow;
        
        var updatedNote = await _noteRepository.UpdateAsync(note, cancellationToken);
        await _noteRepository.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<NoteDto>(updatedNote);
    }
}
