using Microsoft.AspNetCore.Components;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class UsersManagement {
    [Inject]
    public required IUserRoleRepository UserRoleRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private IQueryable<UserRole>? Users { get; set; }

    protected override async Task OnInitializedAsync() {
        var users = await UserRoleRepository.GetAllUsersAsync();

        Users = users.AsQueryable();
    }

    private async Task SetUserRoleAync(string email, UserRoleEnum userRole) {
        var user = await UserRoleRepository.GetUserAsync(email);

        user!.Role = userRole;

        await UserRoleRepository.UpdateUserAsync(user);

        NavigationManager.NavigateTo("/users", true);
    }
    private async Task DeleteUserAsync(string email) {
        await UserRoleRepository.DeleteUserAsync(email);

        NavigationManager.NavigateTo("/users", true);
    }
}
