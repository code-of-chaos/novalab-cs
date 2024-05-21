// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;

namespace NovaLab.Services.Twitch.EventRegistering;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class RegisterCustomRewardRedemption(ILogger logger, NovaLabDbContext dbContext, TwitchAPI twitchApi, TwitchTokensManager twitchAccessToken) {
    public async Task RegisterAtWebSocket(EventSubWebsocketClient client) {
        try {
            // Thanks to Noyainrain for helping me!
            async void Action(NovaLabUser user) {
                string accessToken = await twitchAccessToken.GetAccessTokenOrRefreshAsync(user);

                // subscribe to topics
                // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
                await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.channel_points_custom_reward_redemption.add", "1", new Dictionary<string, string> {
                        // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-points-custom-reward-redemption-add-condition
                        { "broadcaster_user_id", user.TwitchBroadcasterId! },
                    },
                    // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
                    EventSubTransportMethod.Websocket, client.SessionId, null, // Don't set because we are using websocket
                    null, // Don't set because we are using websocket
                    twitchApi.Settings.ClientId, accessToken);
                
                logger.Information("ASSIGNED!");
            }

            await dbContext.Users
                .Where(user => dbContext.TwitchManagedRewards.Any(reward => reward.User == user))
                .ForEachAsync(Action);
        }
        catch (Exception ex){
            logger.Warning(ex, "");
        }
    }
}