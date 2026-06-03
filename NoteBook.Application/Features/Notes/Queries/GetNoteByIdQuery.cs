namespace NoteBook.Application.Features.Notes.Queries;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Query to get a single note by ID
/// </summary>
public record GetNoteByIdQuery(Guid NoteId) : IRequest<NoteDto?>;
