namespace NoteBook.Application.Features.Notes.Queries;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for searching notes by title and content
/// </summary>
public class SearchNotesQueryHandler : IRequestHandler<SearchNotesQuery, IEnumerable<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    public SearchNotesQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<NoteDto>> Handle(SearchNotesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return [];
        
        var notes = await _noteRepository.SearchNotesAsync(request.UserId, request.SearchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<NoteDto>>(notes);
    }
}
