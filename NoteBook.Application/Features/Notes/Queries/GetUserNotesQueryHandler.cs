namespace NoteBook.Application.Features.Notes.Queries;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for retrieving all active notes for a user
/// </summary>
public class GetUserNotesQueryHandler : IRequestHandler<GetUserNotesQuery, IEnumerable<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    public GetUserNotesQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<NoteDto>> Handle(GetUserNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetUserNotesAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<NoteDto>>(notes);
    }
}
