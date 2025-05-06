using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.Diagnostics;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Edit {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [SupplyParameterFromQuery]
    private int Id { get; init; }
    [SupplyParameterFromForm]
    private Quote? Quote { get; set; }

    protected override async Task OnInitializedAsync() {
        Quote = await QuotesRepository.GetQuoteAsync(Id);
    }

    private async Task OnValidSubmitAsync(EditContext editContext) {
        Debug.Assert(Quote is not null);

        await QuotesRepository.UpdateQuoteAsync(Quote);

        NavigationManager.NavigateTo("/quotes");
    }
}
