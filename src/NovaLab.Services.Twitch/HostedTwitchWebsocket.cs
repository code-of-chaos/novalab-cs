// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class HostedTwitchWebsocket(
    ILogger logger,
    EventSubWebsocketClient eventSubWebsocketClient,
    IHttpClientFactory httpClientFactory,
    IServiceScopeFactory scopeFactory
    ) : IHostedService {

    private IServiceScope _scope = null!;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private Task OnErrorOccurred(object sender, ErrorOccuredArgs e) {
        logger.Error("Twitch Websocket {sessionId} - Error occurred! {@e}", eventSubWebsocketClient.SessionId, e);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _scope = scopeFactory.CreateScope();
        var eventCallbacks = new TwitchEventsCallback(_scope);
        
        eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        eventSubWebsocketClient.ChannelPointsCustomRewardRedemptionAdd += eventCallbacks.CatchTwitchManagedReward.Callback;
        
        await eventSubWebsocketClient.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        await eventSubWebsocketClient.DisconnectAsync();
        _scope.Dispose();
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs websocketConnectedArgs) {
        if (websocketConnectedArgs.IsRequestedReconnect) return;
        
        logger.Information($"Websocket {eventSubWebsocketClient.SessionId} connected!");
        
        var eventRegisters = new TwitchRegisterAtWebsocket(_scope);
        await eventRegisters.RegisterCustomRewardRedemption.RegisterAtWebSocket(eventSubWebsocketClient);
    }
    
    private async Task OnWebsocketDisconnected(object sender, EventArgs e) {
        logger.Error($"Websocket {eventSubWebsocketClient.SessionId} disconnected!");

        // Don't do this in production. You should implement a better reconnect strategy
        while (!await eventSubWebsocketClient.ReconnectAsync()) {
            logger.Error("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    private Task OnWebsocketReconnected(object sender, EventArgs e) {
        logger.Warning($"Websocket {eventSubWebsocketClient.SessionId} reconnected");
        return Task.CompletedTask;
    }
}