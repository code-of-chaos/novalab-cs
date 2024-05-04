﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using ChannelFollowArgs = TwitchLib.EventSub.Websockets.Core.EventArgs.Channel.ChannelFollowArgs;

namespace NovaLab.Logic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchWebsocketHostedService : IHostedService {
    private readonly ILogger<TwitchWebsocketHostedService> _logger;
    private readonly EventSubWebsocketClient _eventSubWebsocketClient;

    public TwitchWebsocketHostedService(ILogger<TwitchWebsocketHostedService> logger, EventSubWebsocketClient eventSubWebsocketClient) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _eventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
        _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        _eventSubWebsocketClient.ChannelFollow += OnChannelFollow;
    }

    private async void OnErrorOccurred(object? sender, ErrorOccuredArgs e)
    {
        _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
    }

    private async void OnChannelFollow(object? sender, ChannelFollowArgs channelFollowArgs)
    {
        var eventData = channelFollowArgs.Notification.Payload.Event;
        _logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _eventSubWebsocketClient.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _eventSubWebsocketClient.DisconnectAsync();
    }

    private async void OnWebsocketConnected(object? sender, WebsocketConnectedArgs e)
    {
        _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

        if (!e.IsRequestedReconnect) {
            // subscribe to topics
            
        }
    }

    private async void OnWebsocketDisconnected(object? sender, EventArgs e)
    {
        _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

        // Don't do this in production. You should implement a better reconnect strategy
        while (!await _eventSubWebsocketClient.ReconnectAsync())
        {
            _logger.LogError("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    private async void OnWebsocketReconnected(object? sender, EventArgs e)
    {
        _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
    }
}