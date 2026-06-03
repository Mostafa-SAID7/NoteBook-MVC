namespace NoteBook.Application.Features.Notes.Commands;

using AutoMapper;
using FluentValidation;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Entities;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for creating a new note with validation
/// </summary>
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateNoteCommand> _validator;
    
    public CreateNoteCommandHandler(
        INoteRepository noteRepository, 
        IMapper mapper,
        IValidator<CreateNoteCommand> validator)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
        _validator = validator;
    }
    
    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            Tags = request.Tags,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var createdNote = await _noteRepository.AddAsync(note, cancellationToken);
        await _noteRepository.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<NoteDto>(createdNote);
    }
}
