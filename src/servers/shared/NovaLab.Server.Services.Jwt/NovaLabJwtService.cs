// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames=Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NovaLab.Server.Services.Jwt;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabJwtService(IOptions<JwtSettings> jwtOptions, ILogger logger) {
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private (string Token, DateTime Expiration) GenerateToken(Guid userId, int minutes) {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        DateTime expiration = DateTime.UtcNow.AddMinutes(minutes);
        
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return (Token:new JwtSecurityTokenHandler().WriteToken(token), Expiration:expiration);
    }
    
    public (string Token, DateTime ExpiresAt) GenerateAccessToken(Guid userId) => GenerateToken(userId, 15);
    public (string Token, DateTime ExpiresAt) GenerateRefreshToken(Guid userId) => GenerateToken(userId, 10080); // 7 days

    public bool TryValidateToken(string token, out Guid userId, [NotNullWhen(true)] out ClaimsPrincipal? principal) {
        userId = Guid.Empty;
        principal = null;
        
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey))
        };
        try {
            principal = handler.ValidateToken(token, validationParameters, out _);
            return principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value is {} userIdValue
                   && Guid.TryParse(userIdValue, out userId);
        }
        catch (Exception e) {
            logger.Error(e, "Failed to validate token");
            return false;
        }
    }
}
