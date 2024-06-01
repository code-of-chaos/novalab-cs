// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using TwitchLib.Api.Helix.Models.EventSub;

namespace NovaLab.Hosted.Twitch.Events.CustomRewardRedemption;

using NovaLab.ApiClient.Api;
using NovaLab.ApiClient.Model;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomRewardRedemption(
    ILogger logger, 
    IHttpClientFactory clientFactory,
    TwitchAPI twitchApi, 
    TwitchTokensManager twitchAccessToken
) {
    private HttpClient? _clientCache;
    private HttpClient Client => _clientCache ??=  clientFactory.CreateClient("TwitchServicesClient") ;

    private TwitchManagedRewardApi? _rewardApiCache;
    private TwitchManagedRewardApi RewardApi => _rewardApiCache ??= new TwitchManagedRewardApi(Client.BaseAddress!.ToString());
    
    private ConcurrentBag<string> SubscribedBroadcasters { get; } = [];
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task RegisterAtWebSocket(EventSubWebsocketClient client) {
        // Thanks to Noyainrain for helping me!
        TwitchManagedRewardDtoApiResult? result = await RewardApi.GetManagedRewardsAsync();
        if (result is { Data: null } or null) {
            logger.Warning("API endpoint of GetManagedRewards yielded no result");
            return;
        }
        
        foreach (TwitchManagedRewardDto reward in result.Data) {
            await RegisterSubscription(client, reward);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Support Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task RegisterSubscription(EventSubWebsocketClient client, TwitchManagedRewardDto twitchManagedReward) {
        // Can't subscribe to the same broadcaster multiple times 
        if (SubscribedBroadcasters.Contains(twitchManagedReward.UserId)) {
            logger.Information("Skipping {goal} because the broadcaster is already subscribed to", twitchManagedReward.ManagedRewardId);
            return;
        }
        
        try {
            // stored as a var to make stacktrace a bit clearer
            string accessToken = await twitchAccessToken.GetAccessTokenOrRefreshAsync(twitchManagedReward.UserId);
            
            // subscribe to topics
            // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
            CreateEventSubSubscriptionResponse? result = await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.channel_points_custom_reward_redemption.add",
                "1",
                new Dictionary<string, string> {
                    // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-points-custom-reward-redemption-add-condition
                    { "broadcaster_user_id", twitchManagedReward.TwitchBroadcasterId },
                    // { "reward_id", "..." } // Technically not needed as we want to listen to all rewards and then filter
                },
                // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
                EventSubTransportMethod.Websocket,
                client.SessionId,
                null, // Don't set because we are using websocket
                null, // Don't set because we are using websocket
                twitchApi.Settings.ClientId, 
                accessToken
            );
                    
            if (result is not null && result.Subscriptions.Length != 0) {
                SubscribedBroadcasters.Add(twitchManagedReward.UserId);
            }
        }
        catch (Exception ex){
            logger.Error(ex, "Inner RegisterSubscription Exceptions, {@dto}",twitchManagedReward);
        }
        

    }
    
}