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

        var a = Environment.CurrentDirectory;
    }

    public async Task<string> CheckAndUpdateScheme() {
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

            var sqlSetDbVersion =
                """
                INSERT INTO [Info] (Name, Value) Values ('DbVersion', '1')
                """;

            await connection.ExecuteAsync(sqlSetDbVersion).ConfigureAwait(false);

            version = "1";
        }

        return version;
    }

    public async Task<IEnumerable<Quote>> GetAllQuotesAsync() {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            SELECT Id, Author, Text, Date, Visibility FROM [Quote]            
            """;

        return await connection.QueryAsync<Quote>(sql).ConfigureAwait(false);
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
    public async Task InsertQuotesAsync(IEnumerable<Quote> quotes) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            INSERT INTO [Quote] (Author, Text, Date, Visibility) VALUES (@Author, @Text, @Date, @Visibility)
            """;

        await connection.ExecuteAsync(sql, quotes).ConfigureAwait(false);

        _logger.LogDebug("Quotes inserted ({cnt})", quotes.Count());
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
}
