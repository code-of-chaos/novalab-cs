// // ---------------------------------------------------------------------------------------------------------------------
// // Imports
// // ---------------------------------------------------------------------------------------------------------------------
// using Microsoft.Extensions.DependencyInjection;
// using NovaLab.Server.Services.Twitch.HostedService.Events;
//
// namespace NovaLab.Server.Services.Twitch.HostedService;
//
// // ---------------------------------------------------------------------------------------------------------------------
// // Code
// // ---------------------------------------------------------------------------------------------------------------------
// public static class ServiceExtensions {
//     public static IServiceCollection AddHostedTwitchServices(this IServiceCollection serviceCollection) {
//         serviceCollection.AddHostedService<TwitchEventSubWebsocket>();
//         
//         serviceCollection.AddScoped<CatchTwitchManagedReward>();
//         serviceCollection.AddScoped<CatchTwitchFollow>();
//         serviceCollection.AddScoped<RegisterCustomRewardRedemption>();
//         serviceCollection.AddScoped<RegisterTwitchFollow>();
//         
//         return serviceCollection;
//     } 
// } 
