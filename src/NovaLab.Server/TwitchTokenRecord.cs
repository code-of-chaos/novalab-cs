// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Globalization;
using TwitchLib.Api.Auth;

namespace NovaLab.Server;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record TwitchTokenRecord(
    string AccessToken,
    string RefreshToken,
    string ExpiresAt,
    string TokenType
) {
    public IEnumerable<Tuple<string, string>> GetAsEnumerator() {
        yield return new Tuple<string, string>(TwitchTokensManager.AccessToken, AccessToken);
        yield return new Tuple<string, string>(TwitchTokensManager.RefreshToken, RefreshToken);
        yield return new Tuple<string, string>(TwitchTokensManager.ExpiresAt, ExpiresAt);
        yield return new Tuple<string, string>(TwitchTokensManager.TokenType, TokenType);
    }
    
    public static TwitchTokenRecord CreateFromRefreshResponse(RefreshResponse response, ValidateAccessTokenResponse validation) {
        return new TwitchTokenRecord(
        AccessToken: response.AccessToken, 
        RefreshToken: response.RefreshToken,
        ExpiresAt: DateTime.Now.AddSeconds(validation.ExpiresIn).ToString(CultureInfo.CurrentCulture) ,
        TokenType: "bearer"
        );
    }
}
