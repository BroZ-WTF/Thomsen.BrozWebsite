using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.HttpOverrides;

using System.Net;

using Thomsen.BrozWebsite.Components;
using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite;
public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Services.AddTransient<DbUpdater>();

        builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
        builder.Services.AddTransient<IQuotesRepository, SqliteQuotesRepository>();
        builder.Services.AddTransient<IClaimsTransformation, RoleClaimsTransformer>();

        builder.Services.AddCascadingAuthenticationState();

        builder.Services
            .AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
                options.ClientId = builder.Configuration["Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            RequireHeaderSymmetry = false,
            ForwardLimit = null,
            KnownProxies = { IPAddress.Parse("127.0.0.1") }
        });

        await app.Services.GetRequiredService<DbUpdater>().CheckAndUpdateScheme();

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapGet("/login", async context => {
            await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties {
                RedirectUri = "/"
            });
        });

        app.MapGet("/logout", async context => {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties {
                RedirectUri = "/"
            });
        });

        await app.RunAsync();
    }
}
