using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class UserInfo {
    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; init; }
    [Inject]
    public required IUserRoleRepository UserRoleRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private ClaimsPrincipal? User { get; set; }

    protected override async Task OnInitializedAsync() {
        var authState = await AuthenticationState.GetAuthenticationStateAsync();

        User = authState?.User;
    }

    private async Task DeleteUserAsync() {
        var email = User?.FindFirst(ClaimTypes.Email)?.Value;

        if (email is not null) {
            await UserRoleRepository.DeleteUserAsync(email);

            NavigationManager.NavigateTo("logout", true);
        }
    }
}
