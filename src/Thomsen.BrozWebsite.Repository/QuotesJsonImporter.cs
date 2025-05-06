using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Thomsen.BrozWebsite.Repository;
public class QuotesJsonImporter {
    public static async Task<IEnumerable<Quote>> ImportAsync(string filePath) {
        using var stream = File.OpenRead(filePath);

        return await ImportAsync(stream).ConfigureAwait(false);
    }
    public static async Task<IEnumerable<Quote>> ImportAsync(Stream stream) {
        var quotes = await JsonSerializer.DeserializeAsync<QuotesJson>(stream).ConfigureAwait(false)
            ?? throw new InvalidOperationException("no valid quotes json");

        foreach (var quote in quotes.Quotes) {
            quote.Sanitize();
        }

        return quotes.Quotes;
    }

    private class QuotesJson {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("quotes_list")]
        public required Quote[] Quotes { get; set; }
    }
}
