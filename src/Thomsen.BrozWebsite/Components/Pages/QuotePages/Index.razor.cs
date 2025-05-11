using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Web.Virtualization;

using System.Collections.Immutable;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Security.Claims;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages.QuotePages;
public partial class Index {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }
    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; init; }

    private List<string> ImportJsonErrors { get; } = [];

    private string AuthorFilter { get; set; } = "";
    private string TextFilter { get; set; } = "";
    private PaginationState PaginationState { get; } = new() { ItemsPerPage = 20 };

    private async ValueTask<GridItemsProviderResult<Quote>> LoadQuotesAsync(GridItemsProviderRequest<Quote> request) {
        var quotes = await QuotesRepository.GetAllQuotesAsync();

        var showHidden = await IsAtLeast(UserRoleEnum.Editor);

        var filteredQuotes = quotes
            .Where(quote => showHidden || !quote.Hidden)
            .Where(quote => string.IsNullOrEmpty(AuthorFilter) || quote.Author.Contains(AuthorFilter, StringComparison.InvariantCultureIgnoreCase))
            .Where(quote => string.IsNullOrEmpty(TextFilter) || quote.Text.Contains(TextFilter, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        var orderedQuotes = request.SortByColumn is null
            ? filteredQuotes.OrderByDescending(quote => quote.Date).AsQueryable()
            : request.ApplySorting(filteredQuotes.AsQueryable());

        var pagedQuotes = orderedQuotes
            .Skip(request.StartIndex)
            .Take(request.Count ?? filteredQuotes.Count)
            .ToList();

        return GridItemsProviderResult.From(pagedQuotes, filteredQuotes.Count);
    }

    private async Task ImportJsonFileAsync(InputFileChangeEventArgs e) {
        if (!await IsAtLeast(UserRoleEnum.Editor)) {
            ImportJsonErrors.Add("No rights to do that");
            return;
        }

        ImportJsonErrors.Clear();

        if (e.File.ContentType != "application/json") {
            ImportJsonErrors.Add("File was not a JSON");
            return;
        }
        if (e.FileCount > 1) {
            ImportJsonErrors.Add("More than one file");
            return;
        }
        if (e.FileCount < 0) {
            return;
        }

        using var stream = e.File.OpenReadStream();

        var quotes = await QuotesJsonImporter.ImportAsync(stream);

        await QuotesRepository.InsertQuotesAsync(quotes);

        NavigationManager.NavigateTo("/quotes", true);
    }

    private async Task<bool> IsAtLeast(UserRoleEnum minRole) {
        var authState = await AuthenticationState.GetAuthenticationStateAsync();

        var identity = authState?.User?.Identity as ClaimsIdentity;

        return identity?.HasClaim(claim => claim.Type == ClaimTypes.Role && Enum.Parse<UserRoleEnum>(claim.Value) >= minRole) ?? false;
    }
}
