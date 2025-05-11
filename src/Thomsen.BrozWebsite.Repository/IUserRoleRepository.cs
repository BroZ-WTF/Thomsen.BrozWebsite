
namespace Thomsen.BrozWebsite.Repository;

public interface IUserRoleRepository {
    Task<UserRole?> GetUserAsync(string email);
    Task<UserRole[]> GetAllUsersAsync();
    Task InsertUserAsync(UserRole user);
    Task UpdateUserAsync(UserRole user);
    Task DeleteUserAsync(string email);
}