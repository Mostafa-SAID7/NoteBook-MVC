namespace NoteBook.Application.Features.Notes.Commands;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Command to update an existing note
/// </summary>
public record UpdateNoteCommand(
    Guid NoteId,
    Guid UserId,
    string Title,
    string Content,
    string Tags) : IRequest<NoteDto>;
