// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Components;
using NovaLab.Server.Components.Account;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Account;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace NovaLab.Server;

public static class Program {
    public async static Task Main(string[] args) {
        // -------------------------------------------------------------------------------------------------------------
        // Builder Setup
        // -------------------------------------------------------------------------------------------------------------
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.OverrideLoggingAsSeriLog();
        builder.Configuration.AddEnvironmentVariables(); // Else they won't be loaded

        var environmentSwitcher = new EnvironmentSwitcher(Log.Logger, builder);
        
        // -------------------------------------------------------------------------------------------------------------
        // Services
        // -------------------------------------------------------------------------------------------------------------
        // - Pages & Components-
        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // - Identity & Auth -
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<NovaLabUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<NovaLabDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<NovaLabUser>, IdentityNoOpEmailSender>();

        
        // - Db -
        string connectionString = environmentSwitcher.GetDatabaseConnectionString();
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(connectionString);
        });
        
        // - Kestrel SLL - 
        builder.WebHost.ConfigureKestrel(options => {
            options.ConfigureHttpsDefaults(opt => {
                opt.ServerCertificate = new X509Certificate2( environmentSwitcher.GetSslCertLocation(), environmentSwitcher.GetSslCertPassword());
            });
        });

        // -------------------------------------------------------------------------------------------------------------
        // NovaLabApp
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<NovaLabApp>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        await app.RunAsync().ConfigureAwait(false);
    }
}
