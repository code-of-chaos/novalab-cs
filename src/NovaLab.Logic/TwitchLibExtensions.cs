// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;

namespace NovaLab.Logic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class TwitchLibExtensions {
    public static async Task<(bool IsSuccess, CustomReward[]? CustomRewards )> TryGetCustomRewardAsync(this ChannelPoints channelPoints, string broadcasterId, List<string>? rewardIds = null, bool onlyManageableRewards = false, string? accessToken = null) {
        GetCustomRewardsResponse? result = await channelPoints.GetCustomRewardAsync(broadcasterId, rewardIds, onlyManageableRewards, accessToken);
        return result == null 
            ? (false, null) 
            : (true, result.Data);
    }
}