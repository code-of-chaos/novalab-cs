// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Webhooks.Core;
using TwitchLib.EventSub.Webhooks.Core.EventArgs;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Channel;
using Serilog;

namespace NovaLab.Logic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchEventSubHostedService(ILogger logger, IEventSubWebhooks eventSubWebhooks, IServiceScopeFactory scopeFactory) : IHostedService {
    
    public Task StartAsync(CancellationToken cancellationToken) {
        eventSubWebhooks.OnChannelUpdate += OnChannelUpdate; 
        eventSubWebhooks.OnError += OnError;
        eventSubWebhooks.OnChannelPointsCustomRewardRedemptionAdd += OnChannelPointsCustomRewardRedemptionAdd;
        eventSubWebhooks.OnChannelPointsCustomRewardRedemptionUpdate += OnChannelPointsCustomRewardRedemptionAdd;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        eventSubWebhooks.OnChannelUpdate -= OnChannelUpdate;
        eventSubWebhooks.OnError -= OnError;
        eventSubWebhooks.OnChannelPointsCustomRewardRedemptionAdd -= OnChannelPointsCustomRewardRedemptionAdd;
        eventSubWebhooks.OnChannelPointsCustomRewardRedemptionUpdate -= OnChannelPointsCustomRewardRedemptionAdd;
        return Task.CompletedTask;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Event Methods
    // -----------------------------------------------------------------------------------------------------------------
    private void OnError(object? sender, OnErrorArgs e) {
        logger.Error($"Reason: {e.Reason} - Message: {e.Message}");
    }

    private void OnChannelUpdate(object? sender, ChannelUpdateArgs channelUpdateArgs) {
        logger.Error("{@args}", channelUpdateArgs);
    }
    
    private async void OnChannelPointsCustomRewardRedemptionAdd(object? sender, ChannelPointsCustomRewardRedemptionArgs channelPointsCustomRewardRedemptionArgs) {
        logger.Information("{@rewards}", channelPointsCustomRewardRedemptionArgs);
        
        ChannelPointsCustomRewardRedemption redemption = channelPointsCustomRewardRedemptionArgs.Notification.Event;
        
        // TODO make this into a custom service, so we can tie this to more than just Twitch's API
        using IServiceScope scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
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