using System.Data;

namespace Satlink.Logic;

/// <summary>
/// Factory that creates SQLite connections for Dapper AEMET download repositories.
/// </summary>
public interface ISqliteConnectionFactory
{
    /// <summary>
    /// Creates a new, already-opened SQLite connection.
    /// The caller is responsible for disposing it.
    /// </summary>
    IDbConnection CreateConnection();
}
