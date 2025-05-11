using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class User {
    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; init; }
    [Inject]
    public required IUserRoleRepository UserRoleRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private ClaimsIdentity? ClaimsIdentity { get; set; }

    protected override async Task OnInitializedAsync() {
        var authState = await AuthenticationState.GetAuthenticationStateAsync();

        ClaimsIdentity = authState?.User?.Identity as ClaimsIdentity;
    }

    private async Task DeleteUserAsync() {
        var email = ClaimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (email is not null) {
            await UserRoleRepository.DeleteUserAsync(email);

            NavigationManager.NavigateTo("logout", true);
        }
    }
}
