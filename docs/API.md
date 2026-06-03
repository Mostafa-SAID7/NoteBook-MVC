# API Documentation

## Base URL

```
http://localhost:5000/api
https://localhost:5001/api (with SSL)
```

## Authentication

✅ **JWT Bearer Token** (Implemented)

Include JWT token in Authorization header:
```bash
Authorization: Bearer <token>
```

Get token from `/api/auth/login` endpoint.

**Note**: Health check endpoints are public (no authentication required).

## Response Format

### Success Response
```json
{
  "id": "uuid",
  "data": {}
}
```

### Paginated Response
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 50,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Error Response
```json
{
  "message": "error message",
  "timestamp": "2026-06-03T10:30:00Z",
  "traceId": "trace-id-here",
  "errors": {
    "fieldName": ["error message"]
  }
}
```

---

## Health Check Endpoints

### 1. Full Health Check

**GET** `/health`

Comprehensive health status with dependency checks.

**Authentication**: Not required

**Response** (200 OK):
```json
{
  "status": "Healthy",
  "service": "NoteBook API",
  "timestamp": "2026-06-03T10:30:00Z",
  "version": "2.0.0",
  "environment": "Production",
  "databaseConnected": true,
  "uptimeMs": 3600000
}
```

**Response** (503 Service Unavailable):
```json
{
  "status": "Unhealthy",
  "service": "NoteBook API",
  "timestamp": "2026-06-03T10:30:00Z",
  "version": "2.0.0",
  "environment": "Production",
  "databaseConnected": false,
  "uptimeMs": 3600000
}
```

**Example Request**:
```bash
curl http://localhost:5000/api/health
```

---

### 2. Liveness Probe

**GET** `/health/live`

Quick check that the process is alive (for Kubernetes/Docker).

**Authentication**: Not required

**Response** (200 OK):
```json
{
  "status": "alive",
  "timestamp": "2026-06-03T10:30:00Z"
}
```

**Example Request**:
```bash
curl http://localhost:5000/api/health/live
```

---

### 3. Readiness Probe

**GET** `/health/ready`

Check if service is ready to accept traffic.

**Authentication**: Not required

**Response** (200 OK):
```json
{
  "status": "ready",
  "timestamp": "2026-06-03T10:30:00Z"
}
```

**Response** (503 Service Unavailable):
```json
{
  "status": "not_ready",
  "reason": "Database unavailable",
  "timestamp": "2026-06-03T10:30:00Z"
}
```

**Example Request**:
```bash
curl http://localhost:5000/api/health/ready
```

📖 See [HEALTH_CHECKS.md](HEALTH_CHECKS.md) for detailed health check documentation.

---

## Authentication Endpoints

### Login

**POST** `/auth/login`

Get JWT token for API access.

**Request Body**:
```json
{
  "username": "user@example.com",
  "password": "password"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "username": "user@example.com"
}
```

**Errors**:
- `401 Unauthorized`: Invalid credentials

---

## Notes Endpoints

### 1. Get All Notes (with Pagination)

**GET** `/notes`

Returns all active notes for the current user. Supports optional pagination.

**Authentication**: Required (JWT Bearer Token)

**Query Parameters**:
- `pageNumber` (optional, integer): Page number (1-based)
- `pageSize` (optional, integer): Items per page (1-100)

**Response Without Pagination** (200 OK):
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "First Note",
    "content": "This is the content",
    "tags": "work,important",
    "userId": "550e8400-e29b-41d4-a716-446655440001",
    "createdAt": "2026-06-03T10:30:00Z",
    "updatedAt": "2026-06-03T10:30:00Z"
  }
]
```

**Response With Pagination** (200 OK):
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "First Note",
      "content": "This is the content",
      "tags": "work,important",
      "userId": "550e8400-e29b-41d4-a716-446655440001",
      "createdAt": "2026-06-03T10:30:00Z",
      "updatedAt": "2026-06-03T10:30:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 50,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

**Example Requests**:
```bash
# Get all notes
curl -H "Authorization: Bearer <token>" http://localhost:5000/api/notes

# Get first page with 10 items
curl -H "Authorization: Bearer <token>" "http://localhost:5000/api/notes?pageNumber=1&pageSize=10"

# Get second page
curl -H "Authorization: Bearer <token>" "http://localhost:5000/api/notes?pageNumber=2&pageSize=10"
```

📖 See [PAGINATION.md](PAGINATION.md) for detailed pagination documentation.

---

### 2. Get Note by ID

**GET** `/notes/{id}`

Returns a specific note by its ID.

**Authentication**: Required

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
  "updatedAt": "2026-06-03T10:30:00Z"
}
```

**Errors**:
- `404 Not Found`: Note doesn't exist
- `401 Unauthorized`: Missing or invalid token

**Example Request**:
```bash
curl -H "Authorization: Bearer <token>" http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000
```

---

### 3. Create Note

**POST** `/notes`

Creates a new note for the current user.

**Authentication**: Required

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
  "updatedAt": "2026-06-03T10:30:00Z"
}
```

**Validation**:
- `title`: Required, max 255 characters
- `content`: Required, no length limit
- `tags`: Optional, comma-separated values (max 500 chars)

**Errors**:
- `400 Bad Request`: Validation failed
- `401 Unauthorized`: Missing or invalid token

**Example Request**:
```bash
curl -X POST http://localhost:5000/api/notes \
  -H "Authorization: Bearer <token>" \
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

**Authentication**: Required

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
  "updatedAt": "2026-06-03T11:00:00Z"
}
```

**Errors**:
- `404 Not Found`: Note doesn't exist
- `400 Bad Request`: Validation failed
- `401 Unauthorized`: Missing or invalid token

**Example Request**:
```bash
curl -X PUT http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer <token>" \
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

**Authentication**: Required

**Path Parameters**:
- `id` (UUID): Note ID

**Response** (204 No Content):
```
(empty body)
```

**Errors**:
- `404 Not Found`: Note doesn't exist
- `401 Unauthorized`: Missing or invalid token

**Example Request**:
```bash
curl -X DELETE http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer <token>"
```

---

### 6. Search Notes

**GET** `/notes/search?term=searchterm`

Searches notes by title, content, and tags.

**Authentication**: Required

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
    "updatedAt": "2026-06-03T10:30:00Z"
  }
]
```

**Search Behavior**:
- Case-insensitive
- Searches in: title, content, tags
- Returns active notes only (excludes archived and deleted)

**Errors**:
- `400 Bad Request`: Search term is empty
- `401 Unauthorized`: Missing or invalid token

**Example Request**:
```bash
curl -H "Authorization: Bearer <token>" "http://localhost:5000/api/notes/search?term=work"
```

---

## Status Codes

| Code | Meaning | Common Cause |
|------|---------|-------------|
| 200 | OK | Successful GET/PUT request |
| 201 | Created | Successful POST request |
| 204 | No Content | Successful DELETE request |
| 400 | Bad Request | Invalid input or validation error |
| 401 | Unauthorized | Missing/invalid token |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Unhandled exception |
| 503 | Service Unavailable | Health check failed |

## Rate Limiting

⚠️ **Not implemented** in current version.

**Planned** for production: 100 requests per minute per user.

## Filtering

⚠️ **Not implemented** in current API endpoints.

**Planned**: Filter by date range, tags, archive status.

## Testing Endpoints

### Using cURL

```bash
# Get JWT token
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"user","password":"pass"}' | jq -r '.token')

# Get all notes
curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/notes

# Get with pagination
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5000/api/notes?pageNumber=1&pageSize=10"

# Create note
curl -X POST http://localhost:5000/api/notes \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test","content":"Content","tags":"test"}'

# Update note
curl -X PUT http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated","content":"Updated","tags":"test"}'

# Delete note
curl -X DELETE http://localhost:5000/api/notes/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer $TOKEN"

# Search notes
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5000/api/notes/search?term=test"

# Health check (no token needed)
curl http://localhost:5000/api/health
```

### Using Postman

1. Import API collection
2. Set base URL: `http://localhost:5000/api`
3. Login to get JWT token
4. Add token to Authorization header
5. Run requests

### Using REST Client (VS Code Extension)

Create `.http` file:
```http
@token = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

### Get all notes
GET http://localhost:5000/api/notes
Authorization: Bearer @token

### Get with pagination
GET http://localhost:5000/api/notes?pageNumber=1&pageSize=10
Authorization: Bearer @token

### Create note
POST http://localhost:5000/api/notes
Authorization: Bearer @token
Content-Type: application/json

{
  "title": "Test Note",
  "content": "Test content",
  "tags": "test"
}

### Health check
GET http://localhost:5000/api/health
```

## API Versioning

Current API version: **2.0.0**

API structure supports future versioning with `/api/v2/` path prefix when needed.

## CORS

All origins allowed in development (`*`).

Production: Configure specific allowed origins in `appsettings.json`.

---

**Version**: 2.0.0  
**Last Updated**: June 2026  
**New in 2.1.0**: Health checks, pagination, global error handling
