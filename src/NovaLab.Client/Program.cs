using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Serilog;
using Serilog.Core;

namespace NovaLab.Client;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public async static Task Main(string[] args) {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch())
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .WriteTo.Async(lsc => lsc.Console())
            .CreateLogger();

        builder.Logging.ClearProviders();// Removes the old Microsoft Logging
        builder.Logging.AddSerilog(Log.Logger);
        builder.Services.AddSingleton(Log.Logger);// Else Injecting from Serilog.ILogger won't work

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
        
        builder.Services
            .AddBlazorise( options => {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        await builder.Build().RunAsync();
    }
}
