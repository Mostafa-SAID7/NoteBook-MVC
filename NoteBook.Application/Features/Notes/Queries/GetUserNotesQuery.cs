namespace NoteBook.Application.Features.Notes.Queries;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Query to get all active notes for a user with optional pagination
/// </summary>
public record GetUserNotesQuery(
    Guid UserId, 
    int? PageNumber = null, 
    int? PageSize = null
) : IRequest<object>; // Returns IEnumerable<NoteDto> or PaginatedResponse<NoteDto> based on pagination params
