// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using ChannelPointsCustomRewardRedemptionArgs = TwitchLib.EventSub.Websockets.Core.EventArgs.Channel.ChannelPointsCustomRewardRedemptionArgs;

namespace NovaLab.Logic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchWebsocketHostedService : IHostedService {
    private readonly EventSubWebsocketClient _eventSubWebsocketClient;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TwitchAPI _twitchApi;
    
    public TwitchWebsocketHostedService(ILogger logger,  EventSubWebsocketClient eventSubWebsocketClient, IServiceScopeFactory scopeFactory, TwitchAPI twitchApi) {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _twitchApi = twitchApi;
        
        
        _eventSubWebsocketClient = eventSubWebsocketClient;
        _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        _eventSubWebsocketClient.ChannelPointsCustomRewardRedemptionAdd += OnChannelPointsCustomRewardRedemptionAdd;
        
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private Task OnErrorOccurred(object sender, ErrorOccuredArgs e) {
        _logger.Error("Twitch Websocket {sessionId} - Error occurred! {@e}", _eventSubWebsocketClient.SessionId, e);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        await _eventSubWebsocketClient.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        await _eventSubWebsocketClient.DisconnectAsync();
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e) {
        _logger.Information($"Websocket {_eventSubWebsocketClient.SessionId} connected!");
        using IServiceScope scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        if (!e.IsRequestedReconnect) {
            // Thanks to Noyainrain for helping me!
            await dbContext.Users
                .Where(user => dbContext.TwitchManagedRewards.Any(reward => reward.User == user))
                .ForEachAsync(async user => {
                    IdentityUserToken<string> accessToken = await dbContext.UserTokens
                        .Where(ut => ut.UserId == user.Id && ut.LoginProvider == "Twitch" && ut.Name == "access_token")
                        .FirstAsync();
                   
                    // subscribe to topics
                    // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
                    await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                        "channel.channel_points_custom_reward_redemption.add", 
                        "1", 
                        new Dictionary<string, string> {
                            // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-points-custom-reward-redemption-add-condition
                            {"broadcaster_user_id",  user.TwitchBroadcasterId!},
                            // {"reward_id", null}
                        }, 
                        // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
                        EventSubTransportMethod.Websocket, 
                        // Guid.NewGuid().ToString(),
                        _eventSubWebsocketClient.SessionId,
                        null, // Don't set because we are using websocket
                        null, // Don't set because we are using websocket
                        _twitchApi.Settings.ClientId, 
                        accessToken.Value
                    );
                });
            
        }
    }
    
    private async Task OnWebsocketDisconnected(object sender, EventArgs e) {
        _logger.Error($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

        // Don't do this in production. You should implement a better reconnect strategy
        while (!await _eventSubWebsocketClient.ReconnectAsync()) {
            _logger.Error("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    private Task OnWebsocketReconnected(object sender, EventArgs e) {
        _logger.Warning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
        return Task.CompletedTask;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task OnChannelPointsCustomRewardRedemptionAdd(object sender, ChannelPointsCustomRewardRedemptionArgs pointsCustomRewardRedemptionArgs) {
        _logger.Information("{@rewards}", pointsCustomRewardRedemptionArgs);
        
        ChannelPointsCustomRewardRedemption redemption = pointsCustomRewardRedemptionArgs.Notification.Payload.Event;
        
        // TODO make this into a custom service, so we can tie this to more than just Twitch's API
        using IServiceScope scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        IQueryable<TwitchManagedReward?> queryable = dbContext.TwitchManagedRewards
            .Where(reward => reward.User.TwitchBroadcasterId == redemption.BroadcasterUserId
                             && reward.RewardId == redemption.Reward.Id);

        TwitchManagedReward? twitchManagedReward = await queryable.FirstOrDefaultAsync();
        if (twitchManagedReward == null) return; // No managed reward found
        
        var twitchManagedRewardRedemption = new TwitchManagedRewardRedemption {
            Id = Guid.NewGuid(),
            TwitchManagedReward = twitchManagedReward,
            TimeStamp = DateTime.Now,
            Username = redemption.UserName,
            Message = redemption.UserInput
        };

        dbContext.TwitchManagedRewardRedemptions.Add(twitchManagedRewardRedemption);
        await dbContext.SaveChangesAsync();
    }
}