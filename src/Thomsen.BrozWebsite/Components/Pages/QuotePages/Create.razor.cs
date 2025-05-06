using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Create {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [SupplyParameterFromForm]
    private Quote Quote { get; set; } = new Quote {
        Date = DateTime.Now,
        Visibility = 1
    };

    private async Task OnValidSubmitAsync(EditContext editContext) {
        Quote.Sanitize();

        await QuotesRepository.InsertQuoteAsync(Quote);

        NavigationManager.NavigateTo("/quotes");
    }
}
