
namespace Thomsen.BrozWebsite.Repository;

public interface IUserRoleRepository {
    Task<UserRole?> GetUserAsync(string email);
    Task InsertUserAsync(UserRole user);
}