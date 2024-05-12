// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DependencyInjectionMadeEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using NovaLab.Services.Twitch.Exceptions;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Exceptions;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Support Code
// ---------------------------------------------------------------------------------------------------------------------
public record TwitchTokenRecord(
    IdentityUserToken<string> AccessToken,
    IdentityUserToken<string> RefreshToken,
    IdentityUserToken<string> ExpiresAt,
    IdentityUserToken<string> TokenType
) {
    public IEnumerable<IdentityUserToken<string>> GetAsEnumerator() {
        yield return AccessToken;
        yield return RefreshToken;
        yield return ExpiresAt;
        yield return TokenType;
    }
}

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[DiScoped]
public class TwitchTokensService(ILogger logger, ApplicationDbContext dbContext, TwitchAPI twitchApi, UserManager<ApplicationUser> userManager) {
    private const string AccessToken = "access_token";
    private const string RefreshToken = "refresh_token";
    private const string ExpiresAt = "expires_at";
    private const string TokenType = "token_type";
    private const string Provider = "twitch";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<TwitchTokenRecord> GetTokensAsync(IdentityUser user) {
        IQueryable<IdentityUserToken<string>> tokens = dbContext.UserTokens.Where(token => token.UserId == user.Id && token.LoginProvider == Provider);
        if (await tokens.CountAsync() != 4) {
            throw new AccessTokenCouldNotBeRetrievedException($"Database held no tokens, or the incorrect amount of tokens attached to user {user.Id} ");
        }

        return new TwitchTokenRecord(
            AccessToken : await tokens.FirstAsync(token => token.Name ==AccessToken),
            RefreshToken : await tokens.FirstAsync(token => token.Name == RefreshToken),
            ExpiresAt : await tokens.FirstAsync(token => token.Name == ExpiresAt),
            TokenType : await tokens.FirstAsync(token => token.Name == TokenType)
        );
    }

    private async Task StoreTokensAsync(IdentityUser user, TwitchTokenRecord tokens) {
        List<IdentityUserToken<string>> existingTokens = await dbContext.UserTokens
            .Where(token => token.UserId == user.Id && token.LoginProvider == Provider)
            .ToListAsync();

        foreach (IdentityUserToken<string> newToken in tokens.GetAsEnumerator()) {
            IdentityUserToken<string>? existingToken = existingTokens.FirstOrDefault(t => t.Name == newToken.Name);
            if (existingToken != null) { // Update
                existingToken.Value = newToken.Value;
            }
            else { // add
                dbContext.UserTokens.Add(newToken);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static TwitchTokenRecord CreateTwitchTokens(string userId, string accessToken, string refreshToken, int expiresIn) {
        return new TwitchTokenRecord(
            AccessToken: new IdentityUserToken<string> { UserId = userId, LoginProvider = Provider, Name = AccessToken, Value = accessToken },
            RefreshToken: new IdentityUserToken<string> { UserId = userId, LoginProvider = Provider, Name = RefreshToken, Value = refreshToken },
            ExpiresAt: new IdentityUserToken<string> { UserId = userId, LoginProvider = Provider, Name = ExpiresAt, Value = DateTime.Now.AddSeconds(expiresIn).ToString(CultureInfo.InvariantCulture) },
            TokenType: new IdentityUserToken<string> { UserId = userId, LoginProvider = Provider, Name = TokenType, Value = "bearer" }
        );
    }
    

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<string?> GetAccessTokenOrRefreshAsync(string userId) {
        IdentityUser? user = await dbContext.Users.FindAsync(userId);
        return user is not null 
            ? await GetAccessTokenOrRefreshAsync(user)
            : null;
    }
    public async Task<string?> GetAccessTokenOrRefreshAsync(IdentityUser user) {
        try {
            // Can throw AccessTokenCouldNotBeRetrievedException due to some weirdness if we stored the tokens wrong.
            TwitchTokenRecord tokens = await GetTokensAsync(user);
            
            // Everything is normal and the token should not be refreshed yet
            if (DateTime.Parse(tokens.ExpiresAt.Value!) >= DateTime.Now) return tokens.AccessToken.Value;
            
            // Token has to be refreshed
            if (!await RefreshAccessTokenAsync(user)) return null;
            return (await GetTokensAsync(user)).AccessToken.Value;
        }
        catch (Exception ex) when (ex is BadParameterException or AccessTokenCouldNotBeRetrievedException) {
            logger.Warning(ex, "Access Token could not be retrieved");
            return null;
        }
    }
    
    public async Task<bool> RefreshAccessTokenAsync(string userId) {
        IdentityUser? user = await dbContext.Users.FindAsync(userId);
        return user is not null && await RefreshAccessTokenAsync(user);
    }
    
    public async Task<bool> RefreshAccessTokenAsync(IdentityUser user) {
        try {
            // Refresh the stored tokens
            RefreshResponse? response = await twitchApi.Auth.RefreshAuthTokenAsync( 
                // Can throw AccessTokenCouldNotBeRetrievedException due to some weirdness if we stored the tokens wrong.
                (await GetTokensAsync(user)).RefreshToken.Value,
                twitchApi.Settings.Secret,
                twitchApi.Settings.ClientId
            );

            TwitchTokenRecord newTokens = CreateTwitchTokens(user.Id, response.AccessToken, response.RefreshToken, response.ExpiresIn);
            await StoreTokensAsync(user, newTokens);
            logger.Information("Refreshed Twitch AccessToken for user {id}", user.Id);
            return true;
        }   
        catch (Exception ex) when (ex is BadParameterException or AccessTokenCouldNotBeRetrievedException) {
            logger.Warning(ex, "Access Token could not be retrieved");
            return false;
        }
    }

    public async Task IngestTokensWithUserManager(ApplicationUser user, IEnumerable<AuthenticationToken> authenticationTokens) {
        foreach (AuthenticationToken token in authenticationTokens) {
            // Eh, this is the direct import from the old code, so we might as well keep it this way.
            await userManager.SetAuthenticationTokenAsync(user, Provider, token.Name, token.Value);
        }
    }

    
}