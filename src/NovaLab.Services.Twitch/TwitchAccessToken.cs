// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DependencyInjectionMadeEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using TwitchLib.Api;
using TwitchLib.Api.Auth;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Support Code
// ---------------------------------------------------------------------------------------------------------------------
public record TwitchTokens(
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
public class TwitchAccessToken(ApplicationDbContext dbContext, TwitchAPI twitchApi) {
    private const string AccessToken = "access_token";
    private const string RefreshToken = "refresh_token";
    private const string ExpiresAt = "expires_at";
    private const string TokenType = "token_type";
    private const string Provider = "Twitch";
    

    public async Task<string?> GetAccessTokenAsync(IdentityUser user) {
        TwitchTokens? tokens = await GetTokensAsync(user);
        if (tokens is null) return null;

        if (DateTime.Parse(tokens.ExpiresAt.Value!) >= DateTime.Now) return tokens.AccessToken.Value;
       
        // Refresh the stored tokens
        RefreshResponse? response = await twitchApi.Auth.RefreshAuthTokenAsync( 
            tokens.RefreshToken.Value,
            twitchApi.Settings.Secret,
            twitchApi.Settings.ClientId
        );
        var newTokens = new TwitchTokens(
            AccessToken: new IdentityUserToken<string> { UserId = user.Id, LoginProvider = Provider, Name = AccessToken, Value = response.AccessToken },
            RefreshToken: new IdentityUserToken<string> { UserId = user.Id, LoginProvider = Provider, Name = RefreshToken, Value = response.RefreshToken },
            ExpiresAt: new IdentityUserToken<string> { UserId = user.Id, LoginProvider = Provider, Name = ExpiresAt, Value = DateTime.Now.AddSeconds(response.ExpiresIn).ToString(CultureInfo.InvariantCulture) },
            TokenType: new IdentityUserToken<string> { UserId = user.Id, LoginProvider = Provider, Name = TokenType, Value = "bearer" }
        );
        await StoreTokensAsync(user, newTokens);
        return newTokens.AccessToken.Value;
    }

    private async Task<TwitchTokens?> GetTokensAsync(IdentityUser user) {
        IQueryable<IdentityUserToken<string>> tokens = dbContext.UserTokens.Where(token => token.UserId == user.Id && token.LoginProvider == Provider);
        if (await tokens.CountAsync() != 4) {
            return null; 
        }

        return new TwitchTokens(
            AccessToken : await tokens.FirstAsync(token => token.Name ==AccessToken),
            RefreshToken : await tokens.FirstAsync(token => token.Name == RefreshToken),
            ExpiresAt : await tokens.FirstAsync(token => token.Name == ExpiresAt),
            TokenType : await tokens.FirstAsync(token => token.Name == TokenType)
        );
    }

    private async Task StoreTokensAsync(IdentityUser user, TwitchTokens tokens) {
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
    
}