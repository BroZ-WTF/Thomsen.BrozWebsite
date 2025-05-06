using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Thomsen.BrozWebsite.Repository;
public record Quote {
    [JsonIgnore]
    public required int Id { get; set; }

    [JsonPropertyName("name"), Required, MinLength(3)]
    public required string Author { get; set; }

    [JsonPropertyName("quote"), Required, MinLength(3)]
    public required string Text { get; set; }

    [JsonPropertyName("date"), Required, DataType(DataType.Date)]
    public required DateTime Date { get; set; }

    [JsonPropertyName("visibility"), Required, Range(0, 3)]
    public required int Visibility { get; set; }
}