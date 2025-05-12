using Microsoft.AspNetCore.Components;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class Home {
    [Inject]
    public required IQuotesRepository QuotesRepository { get; init; }
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private ScoreInfo[]? GeneralScores { get; set; }
    private ScoreInfo[]? KonversationScores { get; set; }

    protected override async Task OnInitializedAsync() {
        var quotes = await QuotesRepository.GetAllQuotesAsync();

        var generalScores = new Dictionary<string, int>();
        foreach (var quote in quotes) {
            if (generalScores.TryGetValue(quote.Author, out int cnt)) {
                generalScores[quote.Author] = cnt + 1;
            } else {
                generalScores.Add(quote.Author, 1);
            }
        }

        GeneralScores = generalScores
            .OrderByDescending(score => score.Value)
            .Select(score => new ScoreInfo(score.Key, score.Value))
            .ToArray();

        var konversationScores = new Dictionary<string, int>();
        foreach (var quote in quotes) {
            if (quote.Author == "Konversation") {
                foreach (var author in generalScores.Keys) {
                    if (quote.Text.Contains(author)) {
                        if (konversationScores.TryGetValue(author, out int cnt)) {
                            konversationScores[author] = cnt + 1;
                        } else {
                            konversationScores.Add(author, 1);
                        }
                    }
                }
            }
        }

        KonversationScores = konversationScores
            .OrderByDescending(score => score.Value)
            .Select(score => new ScoreInfo(score.Key, score.Value))
            .ToArray();
    }

    private record ScoreInfo(string User, int Score);
}
