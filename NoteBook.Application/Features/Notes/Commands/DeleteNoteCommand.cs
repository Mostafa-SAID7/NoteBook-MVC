namespace NoteBook.Application.Features.Notes.Commands;

using MediatR;

/// <summary>
/// Command to delete (soft delete) a note
/// </summary>
public record DeleteNoteCommand(Guid NoteId, Guid UserId) : IRequest<bool>;
