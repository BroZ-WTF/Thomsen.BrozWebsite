using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Formatters;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class Home {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }

    private ScoreInfo[]? ScoreInfos { get; set; }

    protected override async Task OnInitializedAsync() {
        var quotes = await QuotesRepository.GetAllQuotesAsync();

        var scores = new Dictionary<string, int>();
        foreach (var quote in quotes) {
            if (scores.TryGetValue(quote.Author, out int value)) {
                scores[quote.Author] = value + 1;
            } else {
                scores.Add(quote.Author, 1);
            }

            if(quote.Author == "Konversation") {

            }
        }

        ScoreInfos = scores
            .OrderByDescending(score => score.Value)
            .Select(score => new ScoreInfo(score.Key, score.Value, 0))
            .ToArray();
    }

    private record ScoreInfo(string User, int QuotesCnt, int CoversationCnt);
}
