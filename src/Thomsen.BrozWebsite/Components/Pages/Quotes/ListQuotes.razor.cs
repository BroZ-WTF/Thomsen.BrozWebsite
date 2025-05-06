using Microsoft.AspNetCore.Components;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.Quotes;
public partial class ListQuotes {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }

    private IEnumerable<Quote>? Quotes { get; set; }

    protected override async Task OnInitializedAsync() {
        Quotes = await QuotesRepository.GetAllQuotesAsync();
    }
}
