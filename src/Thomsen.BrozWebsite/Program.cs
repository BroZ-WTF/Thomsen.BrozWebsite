using System;
using System.Threading.Tasks;

using Thomsen.BrozWebsite.Components;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite;
public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Services.AddTransient<IQuotesRepository, SqliteQuotesRepository>();

        var app = builder.Build();

        // Init Repository/DB
        try {
            var version = await app.Services.GetRequiredService<IQuotesRepository>().CheckAndUpdateScheme();

            app.Logger.LogInformation("Starting with repository version: {version}", version);
        } catch (Exception ex) {
            app.Logger.LogError(ex, "Failed setting up with repository");

            Environment.Exit(1);
        }

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        await app.RunAsync();
    }
}
