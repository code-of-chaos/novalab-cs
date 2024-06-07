// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace CodeOfChaos.Extensions.AspNetCore;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WebApplicationBuilderExtensions {
    public static WebApplicationBuilder OverrideLoggingAsSeriLog(this WebApplicationBuilder builder) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy( new LoggingLevelSwitch())
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .WriteTo.Async(lsc => lsc.Console())
            .CreateLogger();
        
        builder.Logging.ClearProviders(); // Removes the old Microsoft Logging
        builder.Logging.AddSerilog(Log.Logger);
        builder.Services.AddSingleton(Log.Logger); // Else Injecting from Serilog.ILogger won't work
        
        return builder;
    }
}
