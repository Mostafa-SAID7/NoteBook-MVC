namespace NoteBook.Application.Features.Notes.Queries;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for retrieving a single note by ID
/// </summary>
public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto?>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    public GetNoteByIdQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<NoteDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        return _mapper.Map<NoteDto?>(note);
    }
}
