// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using DependencyInjectionMadeEasy;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using Serilog;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace NovaLab.Services.Twitch.EventCallbacks;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[DiScoped]
public class CatchTwitchManagedReward(ILogger logger, ApplicationDbContext dbContext)  {
    public async Task Callback(object sender, ChannelPointsCustomRewardRedemptionArgs pointsCustomRewardRedemptionArgs) {
        logger.Information("{@rewards}", pointsCustomRewardRedemptionArgs);
        
        ChannelPointsCustomRewardRedemption redemption = pointsCustomRewardRedemptionArgs.Notification.Payload.Event;
        
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