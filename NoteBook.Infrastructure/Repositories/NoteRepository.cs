namespace NoteBook.Infrastructure.Repositories;

using Dapper;
using NoteBook.Domain.Entities;
using NoteBook.Domain.Repositories;
using NoteBook.Infrastructure.Data;
using System.Data;

/// <summary>
/// Dapper-based repository implementation for Note entity
/// </summary>
public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private const string TableName = "notes";
    
    public NoteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at, 
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE id = @Id AND is_deleted = false";
        
        var parameters = new { Id = id };
        var note = await connection.QuerySingleOrDefaultAsync<Note>(sql, parameters);
        
        return note;
    }
    
    public async Task<IEnumerable<Note>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE is_deleted = false";
        
        var notes = await connection.QueryAsync<Note>(sql);
        return notes;
    }
    
    public async Task<Note> AddAsync(Note entity, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            INSERT INTO {TableName} 
            (id, title, content, tags, user_id, created_at, updated_at, is_deleted, is_archived)
            VALUES (@Id, @Title, @Content, @Tags, @UserId, @CreatedAt, @UpdatedAt, false, false)";
        
        await connection.ExecuteAsync(sql, entity);
        return entity;
    }
    
    public async Task<Note> UpdateAsync(Note entity, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            UPDATE {TableName}
            SET title = @Title, content = @Content, tags = @Tags, updated_at = @UpdatedAt
            WHERE id = @Id AND is_deleted = false";
        
        await connection.ExecuteAsync(sql, entity);
        return entity;
    }
    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $"DELETE FROM {TableName} WHERE id = @Id";
        var result = await connection.ExecuteAsync(sql, new { Id = id });
        
        return result > 0;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dapper is a micro-ORM and doesn't use a unit of work pattern by default
        // Each operation is immediately committed to the database
        return await Task.FromResult(0);
    }
    
    public async Task<IEnumerable<Note>> GetUserNotesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE user_id = @UserId AND is_deleted = false AND is_archived = false
            ORDER BY updated_at DESC";
        
        var notes = await connection.QueryAsync<Note>(sql, new { UserId = userId });
        return notes;
    }
    
    public async Task<IEnumerable<Note>> GetUserArchivedNotesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE user_id = @UserId AND is_archived = true AND is_deleted = false
            ORDER BY updated_at DESC";
        
        var notes = await connection.QueryAsync<Note>(sql, new { UserId = userId });
        return notes;
    }
    
    public async Task<IEnumerable<Note>> SearchNotesAsync(Guid userId, string searchTerm, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        var searchPattern = $"%{searchTerm}%";
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE user_id = @UserId 
                AND is_deleted = false
                AND (title ILIKE @SearchPattern OR content ILIKE @SearchPattern OR tags ILIKE @SearchPattern)
            ORDER BY updated_at DESC";
        
        var notes = await connection.QueryAsync<Note>(sql, new { UserId = userId, SearchPattern = searchPattern });
        return notes;
    }
    
    public async Task<IEnumerable<Note>> GetNotesByTagAsync(Guid userId, string tag, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        var tagPattern = $"%{tag}%";
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE user_id = @UserId 
                AND is_deleted = false
                AND tags ILIKE @TagPattern
            ORDER BY updated_at DESC";
        
        var notes = await connection.QueryAsync<Note>(sql, new { UserId = userId, TagPattern = tagPattern });
        return notes;
    }
    
    public async Task<(IEnumerable<Note> Items, int Total)> GetUserNotesPagedAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        // Get total count
        const string countSql = $@"
            SELECT COUNT(*) 
            FROM {TableName}
            WHERE user_id = @UserId AND is_deleted = false AND is_archived = false";
        
        var total = await connection.QuerySingleAsync<int>(countSql, new { UserId = userId });
        
        // Get paginated results
        var offset = (page - 1) * pageSize;
        const string sql = $@"
            SELECT id, title, content, tags, user_id, created_at, updated_at,
                   is_deleted, deleted_at, is_archived, archived_at
            FROM {TableName}
            WHERE user_id = @UserId AND is_deleted = false AND is_archived = false
            ORDER BY updated_at DESC
            LIMIT @PageSize OFFSET @Offset";
        
        var notes = await connection.QueryAsync<Note>(sql, 
            new { UserId = userId, PageSize = pageSize, Offset = offset });
        
        return (notes, total);
    }
    
    public async Task<bool> SoftDeleteAsync(Guid noteId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            UPDATE {TableName}
            SET is_deleted = true, deleted_at = @DeletedAt
            WHERE id = @Id";
        
        var result = await connection.ExecuteAsync(sql, 
            new { Id = noteId, DeletedAt = DateTime.UtcNow });
        
        return result > 0;
    }
    
    public async Task<bool> RestoreAsync(Guid noteId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            UPDATE {TableName}
            SET is_deleted = false, deleted_at = NULL
            WHERE id = @Id";
        
        var result = await connection.ExecuteAsync(sql, new { Id = noteId });
        return result > 0;
    }
    
    public async Task<bool> ArchiveAsync(Guid noteId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            UPDATE {TableName}
            SET is_archived = true, archived_at = @ArchivedAt
            WHERE id = @Id AND is_deleted = false";
        
        var result = await connection.ExecuteAsync(sql, 
            new { Id = noteId, ArchivedAt = DateTime.UtcNow });
        
        return result > 0;
    }
    
    public async Task<bool> UnarchiveAsync(Guid noteId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.GetConnection();
        
        const string sql = $@"
            UPDATE {TableName}
            SET is_archived = false, archived_at = NULL
            WHERE id = @Id AND is_deleted = false";
        
        var result = await connection.ExecuteAsync(sql, new { Id = noteId });
        return result > 0;
    }
}
