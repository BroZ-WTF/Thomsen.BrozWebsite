using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.Diagnostics;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.Quotes;
public partial class UpdateQuote {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Parameter]
    public required string Id { get; init; }

    private Quote? Quote { get; set; }

    protected override async Task OnInitializedAsync() {
        if (!int.TryParse(Id, out var id)) {
            throw new InvalidOperationException("id was no valid integer");
        }

        Quote = await QuotesRepository.GetQuoteAsync(id);
    }

    protected async void OnValidSubmitAsync(EditContext editContext) {
        Debug.Assert(Quote is not null);

        await QuotesRepository.UpdateQuoteAsync(Quote);
    }
}
