using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Thomsen.BrozWebsite.Repository;
public record Quote {
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("name"), Required, MinLength(3)]
    public string Author { get; set; } = "";

    [JsonPropertyName("quote"), Required, MinLength(3)]
    public string Text { get; set; } = "";

    [JsonPropertyName("date"), Required, DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [JsonPropertyName("visibility"), Required, Range(0, 3)]
    public int Visibility { get; set; }

    [JsonIgnore]
    public string? Submitter { get; set; }

    [JsonIgnore, Required]
    public bool Hidden {
        get => Visibility > 0;
        set => Visibility = value ? 1 : 0;
    }

    public void Sanitize() {
        Author = Author.Trim();
        Text = Text.Trim();
        Submitter = Submitter?.Trim();
    }
}