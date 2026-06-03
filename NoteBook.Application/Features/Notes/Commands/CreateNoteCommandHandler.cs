namespace NoteBook.Application.Features.Notes.Commands;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Entities;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for creating a new note
/// </summary>
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    public CreateNoteCommandHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
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
