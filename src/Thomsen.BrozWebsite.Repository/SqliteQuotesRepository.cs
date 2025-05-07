using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thomsen.BrozWebsite.Repository;
public class SqliteQuotesRepository : IQuotesRepository {
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public SqliteQuotesRepository(IConfiguration configuration, ILogger<SqliteQuotesRepository> logger) {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("missing connection string 'Default' in configuration");

        _logger = logger;
    }

    public async Task CheckAndUpdateScheme() {
        using var connection = new SqliteConnection(_connectionString);

        var sqlCreateInfoTable =
            """
            CREATE TABLE IF NOT EXISTS [Info] (
             [Name]     TEXT    PRIMARY KEY,
             [Value]    TEXT    NOT NULL
            );
            """;

        await connection.ExecuteAsync(sqlCreateInfoTable).ConfigureAwait(false);

        var sqlGetDbVersion =
            """
            SELECT Value FROM Info WHERE Name = 'DbVersion'
            """;

        var version = await connection.ExecuteScalarAsync<string>(sqlGetDbVersion).ConfigureAwait(false);

        _logger.LogDebug("Schema version is {version}", version);

        // Initial
        if (string.IsNullOrEmpty(version)) {
            var sqlCreateQuoteTable =
                """
                CREATE TABLE IF NOT EXISTS [Quote] (
                    [Id]            INTEGER    PRIMARY KEY,
                    [Author]        TEXT       NOT NULL,
                    [Text]          TEXT       NOT NULL,
                    [Date]          TEXT       NOT NULL,
                    [Visibility]    INT        NOT NULL
                );
                """;

            await connection.ExecuteAsync(sqlCreateQuoteTable).ConfigureAwait(false);

            version = "1";

            var sqlSetDbVersion =
                """
                INSERT INTO [Info] (Name, Value) Values ('DbVersion', @version)
                """;

            await connection.ExecuteAsync(sqlSetDbVersion, new { version }).ConfigureAwait(false);

            _logger.LogDebug("Schema version updated to {version}", version);
        }
    }

    public async Task<Quote> GetQuoteAsync(int id) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            SELECT Id, Author, Text, Date, Visibility FROM [Quote] WHERE Id = @id
            """;

        return await connection.QuerySingleAsync<Quote>(sql, new { id }).ConfigureAwait(false);
    }
    public async Task<Quote[]> GetAllQuotesAsync() {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            SELECT Id, Author, Text, Date, Visibility FROM [Quote]
            """;

        var quotes = await connection.QueryAsync<Quote>(sql).ConfigureAwait(false);

        return quotes.ToArray();
    }

    public async Task InsertQuoteAsync(Quote quote) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            INSERT INTO [Quote] (Author, Text, Date, Visibility) VALUES (@Author, @Text, @Date, @Visibility)
            """;

        await connection.ExecuteAsync(sql, quote).ConfigureAwait(false);

        _logger.LogDebug("Quote inserted: {quote}", quote);
    }
    public async Task InsertQuotesAsync(Quote[] quotes) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            INSERT INTO [Quote] (Author, Text, Date, Visibility) VALUES (@Author, @Text, @Date, @Visibility)
            """;

        await connection.ExecuteAsync(sql, quotes).ConfigureAwait(false);

        _logger.LogDebug("Quotes inserted ({cnt})", quotes.Length);
    }

    public async Task UpdateQuoteAsync(Quote quote) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            UPDATE [Quote] SET Author = @Author, Text = @Text, Date = @Date, Visibility = @Visibility WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, quote).ConfigureAwait(false);

        _logger.LogDebug("Quote update: {quote}", quote);
    }

    public async Task DeleteQuoteAsync(int id) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            DELETE FROM [Quote] WHERE Id = @id
            """;

        await connection.ExecuteAsync(sql, new { id }).ConfigureAwait(false);

        _logger.LogDebug("Quote deleted: {id}", id);
    }
}
