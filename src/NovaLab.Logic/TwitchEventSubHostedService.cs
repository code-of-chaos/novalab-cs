// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Webhooks.Core;
using TwitchLib.EventSub.Webhooks.Core.EventArgs;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Channel;

namespace NovaLab.Logic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchEventSubHostedService(ILogger<TwitchEventSubHostedService> logger, IEventSubWebhooks eventSubWebhooks) : IHostedService {
    
    public Task StartAsync(CancellationToken cancellationToken) {
        eventSubWebhooks.OnError += OnError;
        eventSubWebhooks.OnChannelFollow += OnChannelFollow;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        eventSubWebhooks.OnError -= OnError;
        eventSubWebhooks.OnChannelFollow -= OnChannelFollow;
        return Task.CompletedTask;
    }

    private void OnChannelFollow(object? sender, ChannelFollowArgs e) {
        if (sender is null) return;
        logger.LogInformation($"{e.Notification.Event.UserName} followed {e.Notification.Event.BroadcasterUserName} at {e.Notification.Event.FollowedAt.ToUniversalTime()}");
    }

    private void OnError(object? sender, OnErrorArgs e) {
        if (sender is null) return;
        logger.LogError($"Reason: {e.Reason} - Message: {e.Message}");
    }
}