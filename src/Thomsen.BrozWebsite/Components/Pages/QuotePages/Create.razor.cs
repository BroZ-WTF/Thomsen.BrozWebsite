using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

using System.Security.Claims;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Create {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }
    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; init; }


    [SupplyParameterFromForm]
    private Quote Quote { get; set; } = new Quote {
        Date = DateTime.Now
    };

    private async Task OnValidSubmitAsync(EditContext editContext) {
        var authState = await AuthenticationState.GetAuthenticationStateAsync();

        var email = (authState?.User?.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.Email)?.Value;

        if (!string.IsNullOrEmpty(email)) {
            Quote.Submitter = email;
        }

        Quote.Sanitize();

        await QuotesRepository.InsertQuoteAsync(Quote);

        NavigationManager.NavigateTo("/quotes");
    }
}
