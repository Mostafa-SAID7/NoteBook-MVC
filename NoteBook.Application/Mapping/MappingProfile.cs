namespace NoteBook.Application.Mapping;

using AutoMapper;
using NoteBook.Application.DTOs;
using NoteBook.Domain.Entities;

/// <summary>
/// AutoMapper configuration for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Note mappings
        CreateMap<Note, NoteDto>().ReverseMap();
        CreateMap<CreateOrUpdateNoteRequest, Note>();
        
        // ApplicationUser mappings
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}

/// <summary>
/// DTO for ApplicationUser
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsActive { get; set; }
}
