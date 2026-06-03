namespace NoteBook.Domain.Repositories;

/// <summary>
/// Base repository interface for generic CRUD operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
/// <typeparam name="TId">The primary key type</typeparam>
public interface IRepository<T, TId> where T : class
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
