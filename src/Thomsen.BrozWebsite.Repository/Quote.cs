using System.Text.Json.Serialization;

namespace Thomsen.BrozWebsite.Repository;
public record Quote {
    [JsonIgnore]
    public required int Id { get; set; }
    [JsonPropertyName("name")]
    public required string Author { get; set; }
    [JsonPropertyName("quote")]
    public required string Text { get; set; }
    [JsonPropertyName("date")]
    public required DateTime Date { get; set; }
    [JsonPropertyName("visibility")]
    public required int Visibility { get; set; }
}