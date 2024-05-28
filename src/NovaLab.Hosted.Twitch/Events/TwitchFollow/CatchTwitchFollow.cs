// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch.Events.TwitchFollow;

using NovaLab.ApiClient.Api;
using NovaLab.ApiClient.Model;
using Serilog;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CatchTwitchFollow(ILogger logger, IHttpClientFactory clientFactory) {
    private HttpClient? _clientCache;
    private HttpClient Client => _clientCache ??=  clientFactory.CreateClient("TwitchServicesClient") ;

    private TwitchNewFollowerApi? _newFollowerApi;
    private TwitchNewFollowerApi NewFollowerApi => _newFollowerApi ??= new TwitchNewFollowerApi(Client.BaseAddress!.ToString());
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    public async Task Callback(object sender, ChannelFollowArgs args) {
        // see : https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelfollow
        
        logger.Warning("{@a}", args);
        
        
        await NewFollowerApi.PostNewFollowerAsync(
            new PostNewFollowerDto(
                twitchBoardcasterId: args.Notification.Payload.Event.BroadcasterUserId,
                followerTwitchUserId:args.Notification.Payload.Event.UserId
            )
        ).ConfigureAwait(false);
    }
}