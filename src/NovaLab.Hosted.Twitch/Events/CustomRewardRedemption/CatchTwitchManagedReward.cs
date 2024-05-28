// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch.Events.CustomRewardRedemption;

using NovaLab.ApiClient.Api;
using NovaLab.ApiClient.Model;
using Serilog;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CatchTwitchManagedReward(ILogger logger, IHttpClientFactory clientFactory) {
    private HttpClient? _clientCache;
    private HttpClient Client => _clientCache ??=  clientFactory.CreateClient("TwitchServicesClient") ;

    private TwitchManagedRewardRedemptionApi? _redemptionApiCache;
    private TwitchManagedRewardRedemptionApi RedemptionApi => _redemptionApiCache ??= new TwitchManagedRewardRedemptionApi(Client.BaseAddress!.ToString());
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    // WARN THIS THING HAS TO BE REMADE
    //      JUST SEND ALL DATA TO THE API, fire and forget, maybe pool data together if the callbacks happen in quick succession?
    public async Task Callback(object sender, ChannelPointsCustomRewardRedemptionArgs pointsCustomRewardRedemptionArgs) {
        ChannelPointsCustomRewardRedemption redemption = pointsCustomRewardRedemptionArgs.Notification.Payload.Event;
        var redemptionDto = new TwitchManagedRewardRedemptionDto(
            rewardId: redemption.Reward.Id,
            username: redemption.UserName,
            message: redemption.UserInput
        );

        try {
            CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            await RedemptionApi.PostRedemptionAsync(redemptionDto, cancellationToken: token).ConfigureAwait(false);
        }
        catch (TokenExpiredException exception) {
            logger.Error(exception, "Connection to API could not be established due to expired token.");
        }
        catch (Exception exception)  {
            logger.Error(exception,"An error occurred while trying to connect to the API.");
        }
    }
}