# API Documentation

## Base URL

```
http://localhost:5000/api
https://localhost:5001/api (with SSL)
```

## Authentication

⚠️ **Current Version**: No authentication implemented. All endpoints are public.

**Production**: Will implement JWT or OAuth2 bearer tokens.

## Response Format

### Success Response
```json
{
  "id": "uuid",
  "data": {}
}
```

### Error Response
```json
{
  "error": "error message",
  "details": "additional information"
}
```

## Notes Endpoints

### 1. Get All Notes

**GET** `/notes`

Returns all active notes for the current user.

**Query Parameters**: None

**Response** (200 OK):
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "First Note",
    "content": "This is the content",
    "tags": "work,important",
    "userId": "550e8400-e29b-41d4-a716-446655440001",
    "createdAt": "2026-06-03T10:30:00Z",
    "updatedAt": "2026-06-03T10:30:00Z",
    "isArchived": false,
    "isDeleted": false
  }
]
```

**Example Request**:
```bash
curl -X GET http://localhost:5000/api/notes
```

---

### 2. Get Note by ID

**GET** `/notes/{id}`

Returns a specific note by its ID.

**Path Parameters**:
- `id` (UUID): Note ID

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "First Note",
  "content": "This is the content",
  "tags": "work,important",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "createdAt": "2026-06-03T10:30:00Z",
  "updatedAt": "2026-06-03T10:30:00Z",
  "isArchived": false,
  "isDeleted": false
}
```

**Errors**:
- `404 Not Found`: Note doesn't exist

**Example Request**:
```bash
curl -X GET http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000
```

---

### 3. Create Note

**POST** `/notes`

Creates a new note for the current user.

**Request Body**:
```json
{
  "title": "My New Note",
  "content": "The content of the note",
  "tags": "tag1,tag2,tag3"
}
```

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "My New Note",
  "content": "The content of the note",
  "tags": "tag1,tag2,tag3",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "createdAt": "2026-06-03T10:30:00Z",
  "updatedAt": "2026-06-03T10:30:00Z",
  "isArchived": false,
  "isDeleted": false
}
```

**Validation**:
- `title`: Required, max 255 characters
- `content`: Required, no length limit
- `tags`: Optional, comma-separated values

**Errors**:
- `400 Bad Request`: Validation failed

**Example Request**:
```bash
curl -X POST http://localhost:5000/api/notes \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My New Note",
    "content": "The content",
    "tags": "work"
  }'
```

---

### 4. Update Note

**PUT** `/notes/{id}`

Updates an existing note.

**Path Parameters**:
- `id` (UUID): Note ID

**Request Body**:
```json
{
  "title": "Updated Title",
  "content": "Updated content",
  "tags": "updated,tags"
}
```

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Updated Title",
  "content": "Updated content",
  "tags": "updated,tags",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "createdAt": "2026-06-03T10:30:00Z",
  "updatedAt": "2026-06-03T11:00:00Z",
  "isArchived": false,
  "isDeleted": false
}
```

**Errors**:
- `404 Not Found`: Note doesn't exist
- `403 Forbidden`: User doesn't have permission

**Example Request**:
```bash
curl -X PUT http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Updated Title",
    "content": "Updated content",
    "tags": "updated"
  }'
```

---

### 5. Delete Note (Soft Delete)

**DELETE** `/notes/{id}`

Soft deletes a note (marks as deleted but doesn't remove from database).

**Path Parameters**:
- `id` (UUID): Note ID

**Response** (204 No Content):
```
(empty body)
```

**Errors**:
- `404 Not Found`: Note doesn't exist
- `403 Forbidden`: User doesn't have permission

**Example Request**:
```bash
curl -X DELETE http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000
```

---

### 6. Search Notes

**GET** `/notes/search?term=searchterm`

Searches notes by title, content, and tags.

**Query Parameters**:
- `term` (string, required): Search term (minimum 1 character)

**Response** (200 OK):
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Note about work",
    "content": "This is about the work project",
    "tags": "work,project",
    "userId": "550e8400-e29b-41d4-a716-446655440001",
    "createdAt": "2026-06-03T10:30:00Z",
    "updatedAt": "2026-06-03T10:30:00Z",
    "isArchived": false,
    "isDeleted": false
  }
]
```

**Search Behavior**:
- Case-insensitive
- Searches in: title, content, tags
- Returns active notes only (excludes archived and deleted)

**Errors**:
- `400 Bad Request`: Search term is empty

**Example Request**:
```bash
curl -X GET "http://localhost:5000/api/notes/search?term=work"
```

---

## Status Codes

| Code | Meaning | Common Cause |
|------|---------|-------------|
| 200 | OK | Successful GET/PUT request |
| 201 | Created | Successful POST request |
| 204 | No Content | Successful DELETE request |
| 400 | Bad Request | Invalid input or validation error |
| 404 | Not Found | Resource doesn't exist |
| 403 | Forbidden | User lacks permission |
| 500 | Server Error | Unhandled exception |

## Rate Limiting

⚠️ **Not implemented** in current version.

**Planned** for production: 100 requests per minute per user.

## Pagination

⚠️ **Not implemented** in current API endpoints.

**Planned**: Support `?page=1&pageSize=20` query parameters.

## Filtering

⚠️ **Not implemented** in current API endpoints.

**Planned**: Filter by date range, tags, archive status.

## Testing Endpoints

### Using cURL

```bash
# Get all notes
curl http://localhost:5000/api/notes

# Get single note
curl http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000

# Create note
curl -X POST http://localhost:5000/api/notes \
  -H "Content-Type: application/json" \
  -d '{"title":"Test","content":"Content","tags":"test"}'

# Update note
curl -X PUT http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated","content":"Updated","tags":"test"}'

# Delete note
curl -X DELETE http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000

# Search notes
curl "http://localhost:5000/api/notes/search?term=test"
```

### Using Postman

1. Import API collection
2. Set base URL: `http://localhost:5000/api`
3. Run requests from collection

### Using REST Client (VS Code Extension)

Create `.http` file:
```http
### Get all notes
GET http://localhost:5000/api/notes

### Create note
POST http://localhost:5000/api/notes
Content-Type: application/json

{
  "title": "Test Note",
  "content": "Test content",
  "tags": "test"
}
```

## Versioning

Current API version: **v1** (implicit)

Future versions will use `/api/v2/` path prefix.

## CORS

All origins allowed in development (`*`).

Production: Configure specific allowed origins in `appsettings.json`.

---

**Version**: 1.0.0  
**Last Updated**: June 2026
