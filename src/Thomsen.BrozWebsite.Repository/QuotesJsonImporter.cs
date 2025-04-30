using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Thomsen.BrozWebsite.Repository;
public class QuotesJsonImporter {
    public static async Task<IEnumerable<Quote>> Import(string filePath) {
        using var stream = File.OpenRead(filePath);

        var quotes = await JsonSerializer.DeserializeAsync<QuotesJson>(stream).ConfigureAwait(false)
            ?? throw new InvalidOperationException("no valid quotes json");

        return quotes.Quotes;
    }

    private class QuotesJson {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("quotes_list")]
        public required Quote[] Quotes { get; set; }
    }
}
