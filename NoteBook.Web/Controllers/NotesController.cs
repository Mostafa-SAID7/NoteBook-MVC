using Microsoft.AspNetCore.Mvc;
using MediatR;
using NoteBook.Application.DTOs;
using NoteBook.Application.Features.Notes.Commands;
using NoteBook.Application.Features.Notes.Queries;

namespace NoteBook.Web.Controllers;

/// <summary>
/// Controller for managing notes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotesController> _logger;
    
    // TODO: Replace with actual user ID from claims when authentication is implemented
    private static readonly Guid DefaultUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    
    public NotesController(IMediator mediator, ILogger<NotesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all notes for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetUserNotesQuery(DefaultUserId);
            var notes = await _mediator.Send(query, cancellationToken);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes");
            return StatusCode(500, "Error retrieving notes");
        }
    }
    
    /// <summary>
    /// Get a specific note by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetNote(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetNoteByIdQuery(id);
            var note = await _mediator.Send(query, cancellationToken);
            
            if (note is null)
                return NotFound("Note not found");
            
            return Ok(note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {NoteId}", id);
            return StatusCode(500, "Error retrieving note");
        }
    }
    
    /// <summary>
    /// Create a new note
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NoteDto>> CreateNote(
        [FromBody] CreateOrUpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateNoteCommand(
                DefaultUserId,
                request.Title,
                request.Content,
                request.Tags);
            
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetNote), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note");
            return StatusCode(500, "Error creating note");
        }
    }
    
    /// <summary>
    /// Update an existing note
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<NoteDto>> UpdateNote(
        Guid id,
        [FromBody] CreateOrUpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateNoteCommand(
                id,
                DefaultUserId,
                request.Title,
                request.Content,
                request.Tags);
            
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId}", id);
            return StatusCode(500, "Error updating note");
        }
    }
    
    /// <summary>
    /// Delete a note (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteNoteCommand(id, DefaultUserId);
            var result = await _mediator.Send(command, cancellationToken);
            
            if (!result)
                return NotFound("Note not found");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            return StatusCode(500, "Error deleting note");
        }
    }
    
    /// <summary>
    /// Search notes by term
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> SearchNotes(
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty");
            
            var query = new SearchNotesQuery(DefaultUserId, term);
            var results = await _mediator.Send(query, cancellationToken);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching notes");
            return StatusCode(500, "Error searching notes");
        }
    }
}
