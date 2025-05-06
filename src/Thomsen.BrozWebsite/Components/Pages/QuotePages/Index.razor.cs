using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.Collections.Immutable;
using System.Net.Mime;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Index {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private List<string> ImportErrors { get; } = [];
    private IQueryable<Quote>? Quotes { get; set; }
    private List<(string name, int score)>? AuthorScores { get; set; }

    protected override async Task OnInitializedAsync() {
        var quotes = await QuotesRepository.GetAllQuotesAsync();

        AuthorScores = quotes
            .GroupBy(quote => quote.Author)
            .Select(grp => (name: grp.Key, score: grp.Count()))
            .OrderByDescending(score => score.score)
            .ToList();

        Quotes = quotes.AsQueryable();
    }

    private async Task LoadJsonFilesAsync(InputFileChangeEventArgs e) {
        ImportErrors.Clear();

        if (e.File.ContentType != "application/json") {
            ImportErrors.Add("File was not a JSON");
            return;
        }
        if (e.FileCount > 1) {
            ImportErrors.Add("More than one file");
            return;
        }
        if (e.FileCount < 0) {
            return;
        }

        using var stream = e.File.OpenReadStream();

        var quotes = await QuotesJsonImporter.ImportAsync(stream);

        await QuotesRepository.InsertQuotesAsync(quotes);

        NavigationManager.NavigateTo("/quotes");
    }
}
