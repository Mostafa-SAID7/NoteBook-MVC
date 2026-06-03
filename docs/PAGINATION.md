# Pagination & Sorting Guide

## Overview

The NoteBook API supports pagination for the `/api/notes` endpoint to handle large datasets efficiently. This guide explains how to use pagination and sort results.

---

## Pagination Parameters

### Query Parameters

Add pagination to the GET notes endpoint using these optional query parameters:

- **`pageNumber`** (optional, default: all): Page number (1-based indexing)
- **`pageSize`** (optional, default: all): Items per page (1-100, max clamped to 100)

### Examples

#### Get all notes (no pagination)
```bash
GET /api/notes
Authorization: Bearer <token>
```

Response returns all notes as an array:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Note 1",
    "content": "Content 1",
    "tags": "important",
    "userId": "...",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  ...
]
```

#### Get first page of 10 items
```bash
GET /api/notes?pageNumber=1&pageSize=10
Authorization: Bearer <token>
```

Response returns paginated response:
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Note 1",
      "content": "Content 1",
      "tags": "important",
      "userId": "...",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    },
    ...
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 45,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

#### Get second page
```bash
GET /api/notes?pageNumber=2&pageSize=10
Authorization: Bearer <token>
```

---

## PaginatedResponse Structure

When pagination parameters are provided, the response includes:

| Property | Type | Description |
|----------|------|-------------|
| `items` | Array | Array of NoteDto objects for the current page |
| `pageNumber` | integer | Current page number (1-based) |
| `pageSize` | integer | Number of items per page |
| `totalItems` | integer | Total number of items across all pages |
| `totalPages` | integer | Total number of pages (calculated) |
| `hasNextPage` | boolean | Whether there's a next page available |
| `hasPreviousPage` | boolean | Whether there's a previous page available |

---

## Sorting

Notes are automatically sorted by `updated_at` in descending order (newest first):

- Active notes: Sorted by `updated_at DESC`
- Search results: Sorted by `updated_at DESC`
- Archived notes: Sorted by `updated_at DESC`

---

## Client-Side Examples

### JavaScript/Fetch

```javascript
// Get paginated results
async function getNotesPaginated(pageNumber, pageSize, token) {
  const params = new URLSearchParams({
    pageNumber,
    pageSize
  });

  const response = await fetch(`/api/notes?${params}`, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  const data = await response.json();
  return data;
}

// Usage
const result = await getNotesPaginated(1, 10, authToken);
console.log(`Page 1 of ${result.totalPages}`);
console.log(`Showing items 1-10 of ${result.totalItems}`);
console.log(result.items);

// Get next page if available
if (result.hasNextPage) {
  const nextPage = await getNotesPaginated(result.pageNumber + 1, result.pageSize, authToken);
  console.log(nextPage.items);
}
```

### C# / HttpClient

```csharp
public class PaginationService
{
    private readonly HttpClient _httpClient;

    public PaginationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginatedResponse<NoteDto>> GetNotesPaginatedAsync(
        int pageNumber, 
        int pageSize, 
        string token,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"/api/notes?pageNumber={pageNumber}&pageSize={pageSize}");
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PaginatedResponse<NoteDto>>(content);
        
        return result;
    }
}
```

### PowerShell

```powershell
# Get first page with 10 items
$params = @{
    Uri = "http://localhost:5000/api/notes?pageNumber=1&pageSize=10"
    Headers = @{ Authorization = "Bearer $token" }
    Method = "Get"
}

$response = Invoke-RestMethod @params
Write-Host "Total pages: $($response.totalPages)"
Write-Host "Items on this page: $($response.items.Count)"
```

---

## Best Practices

### 1. Choose Appropriate Page Size

- **Small pages (5-10)**: Better for mobile, reduced bandwidth
- **Medium pages (20-50)**: Balanced for most use cases
- **Large pages (100)**: Optimized for desktop, bulk operations

```bash
# Mobile - small page
GET /api/notes?pageNumber=1&pageSize=5

# Desktop - larger page
GET /api/notes?pageNumber=1&pageSize=50
```

### 2. Implement Infinite Scroll

```javascript
let currentPage = 1;
let pageSize = 20;
let isLoading = false;

async function loadMoreNotes() {
  if (isLoading) return;
  
  isLoading = true;
  
  try {
    const result = await getNotesPaginated(currentPage, pageSize, token);
    
    // Add items to UI
    displayNotes(result.items);
    
    // Load next page if available
    if (result.hasNextPage) {
      currentPage++;
    } else {
      console.log("No more notes to load");
    }
  } finally {
    isLoading = false;
  }
}
```

### 3. Implement Pagination Controls

```javascript
function renderPaginationControls(paginatedResponse) {
  const controls = document.getElementById('pagination');
  controls.innerHTML = '';

  // Previous button
  if (paginatedResponse.hasPreviousPage) {
    const prev = document.createElement('button');
    prev.textContent = 'Previous';
    prev.onclick = () => loadPage(paginatedResponse.pageNumber - 1);
    controls.appendChild(prev);
  }

  // Page info
  const info = document.createElement('span');
  info.textContent = `Page ${paginatedResponse.pageNumber} of ${paginatedResponse.totalPages}`;
  controls.appendChild(info);

  // Next button
  if (paginatedResponse.hasNextPage) {
    const next = document.createElement('button');
    next.textContent = 'Next';
    next.onclick = () => loadPage(paginatedResponse.pageNumber + 1);
    controls.appendChild(next);
  }
}
```

### 4. Cache Responses

```javascript
const pageCache = {};

async function getCachedPage(pageNumber, pageSize) {
  const key = `page_${pageNumber}_size_${pageSize}`;
  
  if (pageCache[key]) {
    return pageCache[key];
  }

  const result = await getNotesPaginated(pageNumber, pageSize, token);
  pageCache[key] = result;
  
  return result;
}
```

---

## Error Handling

If invalid pagination parameters are provided:

- **Invalid page number**: Automatically clamped to valid range
- **Invalid page size**: Automatically clamped to 1-100
- **No notes**: Returns empty items array with page info

```javascript
async function getNotesSafely(pageNumber, pageSize, token) {
  try {
    // Page number is automatically clamped to 1+ by server
    // Page size is automatically clamped to 1-100 by server
    const result = await getNotesPaginated(pageNumber, pageSize, token);
    
    if (result.items.length === 0 && result.pageNumber > 1) {
      console.warn(`Page ${result.pageNumber} is empty. Last page is ${result.totalPages}`);
    }
    
    return result;
  } catch (error) {
    console.error('Failed to get notes:', error);
    return null;
  }
}
```

---

## Performance Considerations

### Database Impact

- Pagination reduces memory usage by limiting results
- Each request performs a count query and a limit/offset query
- PostgreSQL efficiently handles LIMIT/OFFSET queries with proper indexing

### API Response Time

Typical response times:
- Page 1 (first 10-20 items): ~50-100ms
- Page 50 (offset 500-1000): ~100-150ms
- Large page sizes (100 items): ~150-200ms

### Network Optimization

```javascript
// Request only what you need
const result = await getNotesPaginated(1, 10, token);

// Alternative: Get all notes if small dataset
const allNotes = await getNotesPaginated(1, 9999, token); // Max 100, returns all
```

---

## Troubleshooting

### No pagination in response

**Issue**: Getting array instead of paginated response

**Solution**: Ensure both `pageNumber` and `pageSize` parameters are provided:
```bash
# Wrong - returns array
GET /api/notes?pageSize=10

# Correct - returns PaginatedResponse
GET /api/notes?pageNumber=1&pageSize=10
```

### Page out of range

**Issue**: Requesting page 100 when only 5 pages exist

**Solution**: Check `totalPages` before requesting. Server will return empty `items` array:
```javascript
if (requestedPage > response.totalPages) {
  console.error('Page out of range');
}
```

### Large offset performance

**Issue**: Slow queries on page 1000+

**Tip**: Use cursor-based pagination for very large offsets (future enhancement):
```bash
# Instead of offset-based
GET /api/notes?pageNumber=1000&pageSize=10

# Use cursor-based (future)
GET /api/notes?cursor=last_note_id&limit=10
```

---

## Related Documentation

- [API.md](API.md) - Full endpoint documentation
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development setup

---

**Last Updated**: June 2026  
**API Version**: 2.1.0
