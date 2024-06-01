// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task RegisterAtWebSocket(EventSubWebsocketClient client) {
        try {
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
        catch (Exception ex) {
            logger.Warning(ex, "Register Custom Reward Redemption exception");
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Support Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task RegisterSubscription(EventSubWebsocketClient client, TwitchManagedRewardDto twitchManagedReward) {
        try {
            // stored as a var to make stacktrace a bit clearer
            string accessToken = await twitchAccessToken.GetAccessTokenOrRefreshAsync(twitchManagedReward.UserId);
            
            // subscribe to topics
            // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
            await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
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
        }
        catch (Exception ex){
            logger.Warning(ex, "Inner RegisterSubscription Exceptions, {@dto}",twitchManagedReward);
        }

    }
    
}