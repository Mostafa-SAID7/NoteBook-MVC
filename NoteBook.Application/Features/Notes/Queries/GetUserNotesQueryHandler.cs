namespace NoteBook.Application.Features.Notes.Queries;

using AutoMapper;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Repositories;

/// <summary>
/// Handler for retrieving all active notes for a user, with optional pagination
/// </summary>
public class GetUserNotesQueryHandler : IRequestHandler<GetUserNotesQuery, object>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    
    // Default pagination values
    private const int DefaultPageSize = 10;
    
    public GetUserNotesQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<object> Handle(GetUserNotesQuery request, CancellationToken cancellationToken)
    {
        // If pagination parameters are provided, return paginated response
        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            var pageNumber = Math.Max(1, request.PageNumber.Value);
            var pageSize = Math.Clamp(request.PageSize.Value, 1, 100); // Limit max page size to 100
            
            var (notes, total) = await _noteRepository.GetUserNotesPagedAsync(
                request.UserId, 
                pageNumber, 
                pageSize, 
                cancellationToken);
            
            var mappedNotes = _mapper.Map<IEnumerable<NoteDto>>(notes);
            
            return new PaginatedResponse<NoteDto>(mappedNotes, pageNumber, pageSize, total);
        }
        
        // Otherwise, return all notes without pagination
        var allNotes = await _noteRepository.GetUserNotesAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<NoteDto>>(allNotes);
    }
}
