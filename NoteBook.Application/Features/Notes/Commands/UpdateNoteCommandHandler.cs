namespace NoteBook.Application.Features.Notes.Commands;

using AutoMapper;
using FluentValidation;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Exceptions;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for updating an existing note with validation
/// </summary>
public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateNoteCommand> _validator;
    
    public UpdateNoteCommandHandler(
        INoteRepository noteRepository, 
        IMapper mapper,
        IValidator<UpdateNoteCommand> validator)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
        _validator = validator;
    }
    
    public async Task<NoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
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
