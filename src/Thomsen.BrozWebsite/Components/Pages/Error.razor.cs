using Microsoft.AspNetCore.Components;

using System.Diagnostics;

namespace Thomsen.BrozWebsite.Components.Pages;
public partial class Error {
    [CascadingParameter]
    public required HttpContext HttpContext { get; init; }

    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override void OnInitialized() {
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
    }
}
