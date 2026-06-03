namespace NoteBook.Tests.Features.Notes.Commands;

using Moq;
using AutoMapper;
using FluentValidation;
using NoteBook.Application.DTOs;
using NoteBook.Application.Features.Notes.Commands;
using NoteBook.Domain.Entities;
using NoteBook.Domain.Repositories;
using Xunit;

/// <summary>
/// Unit tests for CreateNoteCommandHandler
/// </summary>
public class CreateNoteCommandHandlerTests
{
    private readonly Mock<INoteRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateNoteCommand>> _mockValidator;
    private readonly CreateNoteCommandHandler _handler;

    public CreateNoteCommandHandlerTests()
    {
        _mockRepository = new Mock<INoteRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<CreateNoteCommand>>();
        _handler = new CreateNoteCommandHandler(_mockRepository.Object, _mockMapper.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesAndReturnsNote()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateNoteCommand(userId, "Test Title", "Test Content", "test");
        
        var createdNote = new Note 
        { 
            Id = Guid.NewGuid(),
            Title = command.Title,
            Content = command.Content,
            Tags = command.Tags,
            UserId = userId
        };
        
        var noteDto = new NoteDto 
        { 
            Id = createdNote.Id,
            Title = createdNote.Title,
            Content = createdNote.Content,
            Tags = createdNote.Tags,
            UserId = userId
        };

        _mockValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdNote);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockMapper
            .Setup(x => x.Map<NoteDto>(createdNote))
            .Returns(noteDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(noteDto.Id, result.Id);
        Assert.Equal(noteDto.Title, result.Title);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateNoteCommand(Guid.NewGuid(), "", "Content", "");
        var validationResult = new FluentValidation.Results.ValidationResult(
            new[] { new FluentValidation.Results.ValidationFailure("Title", "Title is required") }
        );

        _mockValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
