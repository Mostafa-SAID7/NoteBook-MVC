namespace NoteBook.Application.Features.Notes.Queries;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Query to get all active notes for a user
/// </summary>
public record GetUserNotesQuery(Guid UserId) : IRequest<IEnumerable<NoteDto>>;
