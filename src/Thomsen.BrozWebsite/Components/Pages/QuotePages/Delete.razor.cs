using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Delete {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [SupplyParameterFromQuery]
    private int Id { get; init; }

    private Quote? Quote { get; set; }

    protected override async Task OnInitializedAsync() {
        Quote = await QuotesRepository.GetQuoteAsync(Id);
    }

    private async Task OnValidSubmitAsync(EditContext editContext) {
        await QuotesRepository.DeleteQuoteAsync(Id);

        NavigationManager.NavigateTo("/quotes");
    }
}
