using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Layout;
public partial class NavMenu {
    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; init; }

    private ClaimsPrincipal? User { get; set; }
    private bool IsAdmin => User?.IsInRole(UserRoleEnum.Admin.ToString()) ?? false;

    protected override async Task OnInitializedAsync() {
        var authState = await AuthenticationState.GetAuthenticationStateAsync();
        
        User = authState?.User;
    }
}
