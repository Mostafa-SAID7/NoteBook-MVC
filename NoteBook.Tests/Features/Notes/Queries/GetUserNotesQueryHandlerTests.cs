namespace NoteBook.Tests.Features.Notes.Queries;

using Moq;
using AutoMapper;
using NoteBook.Application.DTOs;
using NoteBook.Application.Features.Notes.Queries;
using NoteBook.Domain.Entities;
using NoteBook.Domain.Repositories;
using Xunit;

/// <summary>
/// Unit tests for GetUserNotesQueryHandler
/// </summary>
public class GetUserNotesQueryHandlerTests
{
    private readonly Mock<INoteRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetUserNotesQueryHandler _handler;

    public GetUserNotesQueryHandlerTests()
    {
        _mockRepository = new Mock<INoteRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetUserNotesQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ReturnsUserNotes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserNotesQuery(userId);

        var notes = new List<Note>
        {
            new Note { Id = Guid.NewGuid(), Title = "Note 1", Content = "Content 1", UserId = userId },
            new Note { Id = Guid.NewGuid(), Title = "Note 2", Content = "Content 2", UserId = userId }
        };

        var noteDtos = new List<NoteDto>
        {
            new NoteDto { Id = notes[0].Id, Title = notes[0].Title, Content = notes[0].Content, UserId = userId },
            new NoteDto { Id = notes[1].Id, Title = notes[1].Title, Content = notes[1].Content, UserId = userId }
        };

        _mockRepository
            .Setup(x => x.GetUserNotesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notes);

        _mockMapper
            .Setup(x => x.Map<IEnumerable<NoteDto>>(notes))
            .Returns(noteDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(x => x.GetUserNotesAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoNotes_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserNotesQuery(userId);

        _mockRepository
            .Setup(x => x.GetUserNotesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Note>());

        _mockMapper
            .Setup(x => x.Map<IEnumerable<NoteDto>>(It.IsAny<List<Note>>()))
            .Returns(new List<NoteDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
