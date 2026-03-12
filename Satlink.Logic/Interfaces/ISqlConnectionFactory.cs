using System.Data;

namespace Satlink.Logic;

/// <summary>
/// Factory that creates database connections for Dapper query repositories.
/// Abstracting the connection creation keeps the Logic layer database-agnostic.
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Creates a new, already-opened database connection.
    /// The caller is responsible for disposing it.
    /// </summary>
    IDbConnection CreateConnection();
}
