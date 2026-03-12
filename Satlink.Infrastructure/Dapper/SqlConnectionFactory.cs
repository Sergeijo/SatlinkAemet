using System;
using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Satlink.Logic;

namespace Satlink.Infrastructure.Dapper;

/// <summary>
/// Creates <see cref="SqlConnection"/> instances from the configured connection string.
/// </summary>
internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SatlinkApp")
            ?? throw new InvalidOperationException("Connection string 'SatlinkApp' not found.");
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
