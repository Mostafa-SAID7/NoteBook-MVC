namespace NoteBook.Infrastructure.Data;

using Npgsql;
using System.Data;

/// <summary>
/// Factory for creating PostgreSQL database connections using Npgsql
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}

/// <summary>
/// Implementation of database connection factory for PostgreSQL
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
    
    public IDbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
