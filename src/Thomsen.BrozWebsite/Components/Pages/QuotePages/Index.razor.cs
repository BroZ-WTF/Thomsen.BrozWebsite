using Microsoft.AspNetCore.Components;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Index {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }

    private IQueryable<Quote>? Quotes { get; set; }

    protected override async Task OnInitializedAsync() {
        var quotes = await QuotesRepository.GetAllQuotesAsync();

        Quotes = quotes.AsQueryable();
    }
}
