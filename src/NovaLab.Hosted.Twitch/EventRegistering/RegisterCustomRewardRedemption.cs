// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch.EventRegistering;

using ApiClient.Api;
using ApiClient.Model;
using Microsoft.EntityFrameworkCore;
using Data;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;
using NovaLabUser=Data.NovaLabUser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class RegisterCustomRewardRedemption(
    ILogger logger, 
    IHttpClientFactory clientFactory,
    IDbContextFactory<NovaLabDbContext> dbContextFactory, 
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
        await using NovaLabDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
        try {
            // Thanks to Noyainrain for helping me!
            TwitchManagedRewardApiResult result = await RewardApi.GetManagedRewardsAsync();
            if (result is { Data: null } or null) {
                logger.Warning("API endpoint of GetManagedRewards yielded no result");
                return;
            }
            foreach (TwitchManagedReward reward in result.Data) {
                await RegisterSubscription(client, reward, dbContext);
            }
        }
        catch (Exception ex){
            logger.Warning(ex, "");
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Support Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task RegisterSubscription(EventSubWebsocketClient client, TwitchManagedReward twitchManagedReward, NovaLabDbContext dbContext) {
        NovaLabUser? user = await dbContext.Users.Where(user => user.TwitchBroadcasterId == twitchManagedReward.User.TwitchBroadcasterId).FirstOrDefaultAsync();
        if (user is null) {
            logger.Warning("Broadcaster Id {broadcasterId} could not be tied to a NovalabUser", twitchManagedReward.User.TwitchBroadcasterId);
            return;
        }
        
        
        // subscribe to topics
        // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
            "channel.channel_points_custom_reward_redemption.add",
            "1",
            new Dictionary<string, string>() {
                // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-points-custom-reward-redemption-add-condition
                { "broadcaster_user_id", user.TwitchBroadcasterId! },
                // { "reward_id", "..." } // Technically not needed as we want to listen to all rewards and then filter
            },
            // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
            EventSubTransportMethod.Websocket,
            client.SessionId,
            null, // Don't set because we are using websocket
            null, // Don't set because we are using websocket
            twitchApi.Settings.ClientId, 
            await twitchAccessToken.GetAccessTokenOrRefreshAsync(user.Id)
        );
                
        logger.Information("ASSIGNED!");
    }
    
}