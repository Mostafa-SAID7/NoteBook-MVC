namespace NoteBook.Application.Features.Notes.Queries;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Query to search notes by title and content
/// </summary>
public record SearchNotesQuery(Guid UserId, string SearchTerm) : IRequest<IEnumerable<NoteDto>>;
