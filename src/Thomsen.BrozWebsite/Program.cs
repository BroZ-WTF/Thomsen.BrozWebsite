using System;
using System.Net.Security;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using Thomsen.BrozWebsite.Components;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite;
public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Services.AddTransient<IQuotesRepository, SqliteQuotesRepository>();

        builder.Services.AddCascadingAuthenticationState();

        builder.Services
            .AddAuthentication(options => {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "Google";
            })
            .AddCookie("Cookies")
            .AddGoogle("Google", options => {
                options.ClientId = builder.Configuration["Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        await app.Services.GetRequiredService<IQuotesRepository>().CheckAndUpdateScheme();

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapGet("/login", async context => {
            await context.ChallengeAsync("Google", new AuthenticationProperties {
                RedirectUri = "/"
            });
        });

        app.MapGet("/logout", async context => {
            await context.SignOutAsync("Cookies");
            context.Response.Redirect("/");
        });

        await app.RunAsync();
    }
}
