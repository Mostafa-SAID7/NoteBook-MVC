namespace NoteBook.Application.Features.Notes.Commands;

using MediatR;
using NoteBook.Application.DTOs;

/// <summary>
/// Command to create a new note
/// </summary>
public record CreateNoteCommand(
    Guid UserId,
    string Title,
    string Content,
    string Tags) : IRequest<NoteDto>;
