// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.Hub;
using Serilog;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace NovaLab.Services.Twitch.EventCallbacks;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class CatchTwitchManagedReward(ILogger logger, ApplicationDbContext dbContext, IHubContext<TwitchHub> hubContext, IHttpClientFactory clientFactory) {
    private HttpClient? _clientCache;
    private HttpClient Client => _clientCache ??=  clientFactory.CreateClient("TwitchServicesClient") ;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task Callback(object sender, ChannelPointsCustomRewardRedemptionArgs pointsCustomRewardRedemptionArgs) {
        logger.Information("{@rewards}", pointsCustomRewardRedemptionArgs);
        ChannelPointsCustomRewardRedemption redemption = pointsCustomRewardRedemptionArgs.Notification.Payload.Event;
        IQueryable<TwitchManagedReward> queryable = dbContext.TwitchManagedRewards
            .Where(reward => reward.User.TwitchBroadcasterId == redemption.BroadcasterUserId
                             && reward.RewardId == redemption.Reward.Id)
            .Include(twitchManagedReward => twitchManagedReward.User);

        TwitchManagedReward? twitchManagedReward = await queryable.FirstOrDefaultAsync();
        if (twitchManagedReward == null) {
            logger.Warning("No managed reward found");
            return; // No managed reward found
        }
        
        var twitchManagedRewardRedemptionDto = new TwitchManagedRewardRedemptionDto(
            twitchManagedReward.RewardId,
            redemption.UserName,
            redemption.UserInput
        );
        try 
        {
            CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            HttpResponseMessage response = await Client.PostAsync(
                $"api/{twitchManagedReward.User.Id}/twitch/managed-rewards-redemptions/",
                new StringContent(JsonConvert.SerializeObject(twitchManagedRewardRedemptionDto), Encoding.UTF8,
                    "application/json"
                ),
                token
            );
            
            if(response.IsSuccessStatusCode) {
                logger.Information("API request was successful. Status code: {StatusCode}", response.StatusCode);
            }
            else {
                logger.Warning("API request failed. Status code: {StatusCode}", response.StatusCode);
            }
            
        }
        catch (TokenExpiredException exception) {
            logger.Error(exception, "Connection to API could not be established due to expired token.");
        }
        catch (Exception exception)  {
            logger.Error(exception,"An error occurred while trying to connect to the API.");
        }
        
    }
}