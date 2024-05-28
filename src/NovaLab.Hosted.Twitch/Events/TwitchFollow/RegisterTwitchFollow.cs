// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch.Events.TwitchFollow;

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using NovaLab.ApiClient.Api;
using NovaLab.ApiClient.Model;
using NovaLab.Data;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[UsedImplicitly]
public class RegisterTwitchFollow(
    ILogger logger, 
    IHttpClientFactory clientFactory,
    IDbContextFactory<NovaLabDbContext> dbContextFactory, 
    TwitchAPI twitchApi, 
    TwitchTokensManager twitchAccessToken
) {
    private HttpClient? _clientCache;
    private HttpClient Client => _clientCache ??=  clientFactory.CreateClient("TwitchServicesClient") ;

    private TwitchFollowerGoalApi? _followerGoalApi;
    private TwitchFollowerGoalApi FollowerGoalApi => _followerGoalApi ??= new TwitchFollowerGoalApi(Client.BaseAddress!.ToString());
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task RegisterAtWebSocket(EventSubWebsocketClient client) {
        await using NovaLabDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
        try {
            FollowerGoalDtoApiResult result = await FollowerGoalApi.GetFollowerGoalsAsync();
            if (result is { Data: null } or null) {
                logger.Warning("API endpoint of GetFollowerGoalsAsync yielded no result");
                return;
            }
            foreach (FollowerGoalDto followerGoal in result.Data) {
                await RegisterSubscription(client, followerGoal);
            }
        }
        catch (Exception ex){
            logger.Warning(ex, "");
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Support Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task RegisterSubscription(EventSubWebsocketClient client, FollowerGoalDto followerGoal) {
        // subscribe to topics
        // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelfollow
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
            "channel.follow",
            "2",
            new Dictionary<string, string> {
                // see : https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-follow-condition
                { "broadcaster_user_id", followerGoal.TwitchBroadcasterId },
                { "moderator_user_id", followerGoal.TwitchBroadcasterId },
            },
            // see: https://dev.twitch.tv/docs/eventsub/eventsub-reference/#transport
            EventSubTransportMethod.Websocket,
            client.SessionId,
            null, // Don't set because we are using websocket
            null, // Don't set because we are using websocket
            twitchApi.Settings.ClientId, 
            await twitchAccessToken.GetAccessTokenOrRefreshAsync(followerGoal.UserId)
        );
    }
    
}