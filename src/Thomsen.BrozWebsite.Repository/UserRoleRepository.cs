using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thomsen.BrozWebsite.Repository;
public class UserRoleRepository : IUserRoleRepository {
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public UserRoleRepository(IConfiguration configuration, ILogger<SqliteQuotesRepository> logger) {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("missing connection string 'Default' in configuration");

        _logger = logger;
    }

    public async Task<UserRole?> GetUserAsync(string email) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            SELECT Id, Email, Role FROM [User] WHERE Email = @email
            """;

        return await connection.QuerySingleOrDefaultAsync<UserRole>(sql, new { email }).ConfigureAwait(false);
    }
    public async Task<UserRole[]> GetAllUsersAsync() {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            SELECT Id, Email, Role FROM [User]
            """;

        var users = await connection.QueryAsync<UserRole>(sql).ConfigureAwait(false);

        return users.ToArray();
    }

    public async Task InsertUserAsync(UserRole user) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            INSERT INTO [User] (Email, Role) VALUES (@Email, @Role)
            """;

        await connection.ExecuteAsync(sql, user).ConfigureAwait(false);

        _logger.LogDebug("User inserted: {user}", user);
    }

    public async Task UpdateUserAsync(UserRole user) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """
            UPDATE [User] SET Email = @Email, Role = @Role WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, user).ConfigureAwait(false);

        _logger.LogDebug("User update: {user}", user);
    }

    public async Task DeleteUserAsync(string email) {
        using var connection = new SqliteConnection(_connectionString);

        var sql =
            """            
            DELETE FROM [User] WHERE Email = @email
            """;

        var cnt = await connection.ExecuteAsync(sql, new { email }).ConfigureAwait(false);

        _logger.LogDebug("User deleted: {email}", email);
    }
}
