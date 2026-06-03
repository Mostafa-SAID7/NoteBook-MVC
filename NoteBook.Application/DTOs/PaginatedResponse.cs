namespace NoteBook.Application.DTOs;

/// <summary>
/// Generic paginated response wrapper
/// </summary>
public class PaginatedResponse<T>
{
    /// <summary>
    /// The paginated data items
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalItems { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
    
    /// <summary>
    /// Is there a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// Is there a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    public PaginatedResponse() { }
    
    public PaginatedResponse(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
    }
}
