// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using DependencyInjectionMadeEasy;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;

namespace NovaLab.Services.Twitch.EventRegistering;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[DiScoped]
public class ChannelPointsCustomRewardRedemptionAdd(ILogger logger, ApplicationDbContext dbContext, TwitchAPI twitchApi, TwitchTokensService twitchAccessToken) {
    public async Task RegisterAtWebSocket(EventSubWebsocketClient client) {
        try {
            // Thanks to Noyainrain for helping me!
            await dbContext.Users
                .Where(user => dbContext.TwitchManagedRewards.Any(reward => reward.User == user))
                .ForEachAsync(async user => {
                    string? accessToken = await twitchAccessToken.GetAccessTokenOrRefreshAsync(user);
                    if (accessToken is null) return;
                 
                    // subscribe to topics
                    // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
                    await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                        "channel.channel_points_custom_reward_redemption.add",
                        "1",
                        new Dictionary<string, string> {
                            // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-points-custom-reward-redemption-add-condition
                            { "broadcaster_user_id", user.TwitchBroadcasterId! },
                            // {"reward_id", null}
                        },
                        // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
                        EventSubTransportMethod.Websocket,
                        // Guid.NewGuid().ToString(),
                        client.SessionId,
                        null, // Don't set because we are using websocket
                        null, // Don't set because we are using websocket
                        twitchApi.Settings.ClientId,
                        accessToken
                    );
                });
        }
        catch (Exception ex){
            logger.Warning(ex, "");
        }
    }
}