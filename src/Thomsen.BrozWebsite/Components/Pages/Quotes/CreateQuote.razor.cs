using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.Quotes;
public partial class CreateQuote {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }

    private Quote Quote { get; set; } = new Quote {
        Id = 0,
        Author = "",
        Text = "",
        Date = DateTime.Now,
        Visibility = 1
    };

    protected async void OnValidSubmitAsync(EditContext editContext) {
        await QuotesRepository.InsertQuoteAsync(Quote);
    }
}
