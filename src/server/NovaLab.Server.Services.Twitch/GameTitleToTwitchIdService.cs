// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Database;
using NovaLab.Server.Database.Models.Streams.HelixApi;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;

namespace NovaLab.Server.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GameTitleToTwitchIdService(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    TwitchAPI twitchApi
) : AbstractBaseApiService<NovaLabDbContext>(contextFactory) {

    private ConcurrentDictionary<string, GameTitleToTwitchId>? _fastCache ;
    private ConcurrentDictionary<string, GameTitleToTwitchId> FastCache => _fastCache ??= new ConcurrentDictionary<string, GameTitleToTwitchId>();
    
    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private bool TryGetFromFastCache(string gameTitle, [NotNullWhen(true)] out GameTitleToTwitchId? fastCached) => FastCache.TryGetValue(gameTitle, out fastCached);
    private void AddToFastCache(string name, GameTitleToTwitchId twitchGame) => FastCache.AddOrUpdate(name, twitchGame, (_, _) => twitchGame );

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [SuppressMessage("ReSharper", "UnusedMember.Global")] 
    public void InvalidateCache() => _fastCache = null; // In the future, when the cache gets too big, clear the cache

    public async Task<GameTitleToTwitchId?> GetCategoryByIdAsync(string gameId) {
        await using NovaLabDbContext dbContext = await DbContext;
        return await dbContext.GameTitleToTwitchIds.FirstOrDefaultAsync(item => item.TwitchTitleId == gameId);
    }
    
    public async Task<GameTitleToTwitchId?> GetCategoryByNameAsync(string gameTitle) {
        await using NovaLabDbContext dbContext = await DbContext;
        
        // First try and hit the "fast cache"
        if (TryGetFromFastCache(gameTitle, out GameTitleToTwitchId? fastCachedHit)) {
            logger.Information("Twitch Category of name {name} found in cache with following Twitch Category id {id}, hit from fast cache.", gameTitle, fastCachedHit.TwitchTitleId);
            return fastCachedHit;
        }
        
        // Second try the "slow cache, aka the db"
        GameTitleToTwitchId? cacheHit = await dbContext.GameTitleToTwitchIds.FirstOrDefaultAsync(cache => cache.NovaLabName == gameTitle);
        if (cacheHit is not null) {
            AddToFastCache(gameTitle, cacheHit);
            logger.Information("Twitch Category of name {name} found in cache with following Twitch Category id {id}", gameTitle, cacheHit.TwitchTitleId);
            return cacheHit;
        }
        
        // Third potential try is finding it at twitch themselves
        GetGamesResponse? response = await twitchApi.Helix.Games.GetGamesAsync(gameNames: [gameTitle]);
        Game? game = response?.Games.FirstOrDefault();
        if (game is null) {
            logger.Warning("No Titch Category of name {name} found", gameTitle);
            return null;
        }
        
        // New game was found, so we store in db for "caching"
        var newItem = new GameTitleToTwitchId {
            NovaLabName = gameTitle,
            TwitchTitleId = game.Id,
            TwitchTitleName = game.Name,
            TwitchTitleBoxArtUrl = game.BoxArtUrl,
            TwitchTitleIgdbId = null
        };
        
        // Store item to caches
        AddToFastCache(gameTitle, newItem);
        await dbContext.GameTitleToTwitchIds.AddAsync(newItem);
        await dbContext.SaveChangesAsync();

        return newItem;
    }
}
