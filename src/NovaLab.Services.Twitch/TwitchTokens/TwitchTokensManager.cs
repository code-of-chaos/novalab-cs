// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NovaLab.Data;
using NovaLab.Services.Twitch.Exceptions;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Exceptions;

namespace NovaLab.Services.Twitch.TwitchTokens;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchTokensManager(ILogger logger, TwitchAPI twitchApi, UserManager<ApplicationUser> userManager) {
    internal const string AccessToken = "access_token";
    internal const string RefreshToken = "refresh_token";
    internal const string ExpiresAt = "expires_at";
    internal const string TokenType = "token_type";
    private const string Provider = "twitch";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<TwitchTokenRecord> GetTokensAsync(ApplicationUser user) {
        var ex =new AccessTokenCouldNotBeRetrievedException($"Database held no tokens, or the incorrect amount of tokens attached to user {user.Id} ");

        return new TwitchTokenRecord(
            AccessToken : await userManager.GetAuthenticationTokenAsync(user, Provider, AccessToken) ?? throw ex,
            RefreshToken : await userManager.GetAuthenticationTokenAsync(user, Provider, RefreshToken) ?? throw ex,
            ExpiresAt : await userManager.GetAuthenticationTokenAsync(user, Provider, ExpiresAt) ?? throw ex,
            TokenType : await userManager.GetAuthenticationTokenAsync(user, Provider, TokenType) ?? throw ex
        );
    }

    private async Task StoreTokensAsync(ApplicationUser user, TwitchTokenRecord tokens) {
        foreach ((string tokenName, string tokenValue) in tokens.GetAsEnumerator()) {
            await userManager.SetAuthenticationTokenAsync(user, Provider, tokenName, tokenValue);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<string?> GetAccessTokenOrRefreshAsync(ApplicationUser user) {
        try {
            // Can throw AccessTokenCouldNotBeRetrievedException due to some weirdness if we stored the tokens wrong.
            TwitchTokenRecord tokens = await GetTokensAsync(user);
            
            // Everything is normal and the token should not be refreshed yet
            if (DateTime.Parse(tokens.ExpiresAt) >= DateTime.Now) return tokens.AccessToken;
            
            // Token has to be refreshed
            if (!await RefreshAccessTokenAsync(user)) return null;
            return (await GetTokensAsync(user)).AccessToken;
        }
        catch (Exception ex) when (ex is BadParameterException or AccessTokenCouldNotBeRetrievedException) {
            logger.Warning(ex, "Access Token could not be retrieved");
            return null;
        }
    }
   
    public async Task<bool> RefreshAccessTokenAsync(ApplicationUser user) {
        try {
            // Refresh the stored tokens
            RefreshResponse? response = await twitchApi.Auth.RefreshAuthTokenAsync( 
                // Can throw AccessTokenCouldNotBeRetrievedException due to some weirdness if we stored the tokens wrong.
                (await GetTokensAsync(user)).RefreshToken,
                twitchApi.Settings.Secret,
                twitchApi.Settings.ClientId
            );

            ValidateAccessTokenResponse validation = await twitchApi.Auth.ValidateAccessTokenAsync(response.AccessToken);
            if (validation == null) return false; // I HATE TWITCHLIB
            
            logger.Warning("{@r}", validation);
            logger.Warning("{@r}", response);
            
            await StoreTokensAsync(user, TwitchTokenRecord.CreateFromRefreshResponse(response, validation));
            
            logger.Information("Refreshed Twitch AccessToken for user {id}", user.Id);
            return true;
        }   
        
        catch (Exception ex) when (ex is BadParameterException or AccessTokenCouldNotBeRetrievedException) {
            logger.Warning(ex, "Access Token could not be retrieved");
            return false;
        }
    }

    public async Task IngestTokensWithUserManager(ApplicationUser user, IEnumerable<AuthenticationToken> authenticationTokens) {
        IEnumerable<AuthenticationToken> enumerable = authenticationTokens as AuthenticationToken[] ?? authenticationTokens.ToArray();
        
        foreach (string name in (string[])[AccessToken, RefreshToken, ExpiresAt, TokenType]) {
            AuthenticationToken token = enumerable.First(token => token.Name == name);
            await userManager.SetAuthenticationTokenAsync(user, Provider, token.Name, token.Value);
        }
    }

    
}