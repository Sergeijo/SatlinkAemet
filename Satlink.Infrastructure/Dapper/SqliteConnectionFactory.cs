using System;
using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

using Satlink.Logic;

namespace Satlink.Infrastructure.Dapper;

/// <summary>
/// Creates <see cref="SqliteConnection"/> instances pointing to the AEMET downloads database.
/// </summary>
internal sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AemetDownloads")
            ?? "Data Source=aemet_downloads.db";
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
