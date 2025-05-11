using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Thomsen.BrozWebsite.Repository;
public class DbUpdater {
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public DbUpdater(IConfiguration configuration, ILogger<SqliteQuotesRepository> logger) {
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

        // Initial: Quote table
        if (string.IsNullOrEmpty(version)) {
            var sqlCreateQuoteTable =
                """
                CREATE TABLE IF NOT EXISTS [Quote] (
                    [Id]            INTEGER    PRIMARY KEY,
                    [Author]        TEXT       NOT NULL,
                    [Text]          TEXT       NOT NULL,
                    [Date]          TEXT       NOT NULL,
                    [Visibility]    INTEGER    NOT NULL
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

        // User table
        if (version == "1") {
            var sqlCreateUserTable =
                """
                CREATE TABLE IF NOT EXISTS [User] (
                    [Id]            INTEGER    PRIMARY KEY,
                    [Email]         TEXT       NOT NULL,
                    [Role]          INTEGER    NOT NULL
                );
                """;

            await connection.ExecuteAsync(sqlCreateUserTable).ConfigureAwait(false);

            version = "2";

            await UpdateDbVersionAsync(connection, version).ConfigureAwait(false);
        }

        // Quote table with submitter
        if (version == "2") {
            var sqlUpdateQuoteTable =
                """
                ALTER TABLE [Quote] ADD [Submitter] TEXT NULL;
                """;

            await connection.ExecuteAsync(sqlUpdateQuoteTable).ConfigureAwait(false);

            version = "3";

            await UpdateDbVersionAsync(connection, version).ConfigureAwait(false);
        }
    }

    private async Task UpdateDbVersionAsync(SqliteConnection connection, string version) {
        var sqlSetDbVersion =
            """
            UPDATE [Info] SET Value = @version WHERE Name = 'DbVersion'
            """;

        await connection.ExecuteAsync(sqlSetDbVersion, new { version }).ConfigureAwait(false);

        _logger.LogDebug("Schema version updated to {version}", version);
    }
}
