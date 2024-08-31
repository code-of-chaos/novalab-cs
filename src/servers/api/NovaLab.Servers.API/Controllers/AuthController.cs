// // ---------------------------------------------------------------------------------------------------------------------
// // Imports
// // ---------------------------------------------------------------------------------------------------------------------
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.Tokens;
// using NovaLab.Database.Models.Account;
// using NovaLab.Server.Database.Models.Account;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
//
// namespace NovaLab.Servers.API.Controllers;
//
// // ---------------------------------------------------------------------------------------------------------------------
// // Code
// // ---------------------------------------------------------------------------------------------------------------------
// [ApiController]
// [Route("api/[controller]")]
// public class AuthController(
//     IOptions<JwtSettings> jwtSettings,
//     UserManager<NovaLabUser> userManager,
//     SignInManager<NovaLabUser> signInManager
// ) : ControllerBase {
//     private readonly JwtSettings _jwtSettings = jwtSettings.Value;
//
//     [HttpPost("login")]
//     public async Task<IActionResult> Login([FromBody] LoginRequest request) {
//         if (await userManager.FindByNameAsync(request.Username) is not {} user)
//             return Unauthorized("Invalid username or password");
//         
//         if (!await signInManager.CheckPasswordSignInAsync(user, request.Password))
//         
//         
//         string token = GenerateJwtToken(request.Username);
//         return Ok(new { Token = token });
//     }
//
//     private string GenerateJwtToken(string username) {
//         var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
//         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
//
//         var claims = new[] {
//             new Claim(JwtRegisteredClaimNames.Sub, username),
//             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//             new Claim(ClaimTypes.Role, "User") // Add roles if needed
//         };
//
//         var token = new JwtSecurityToken(
//             issuer: _jwtSettings.Issuer,
//             audience: _jwtSettings.Audience,
//             claims: claims,
//             expires: DateTime.UtcNow.AddHours(1),
//             signingCredentials: credentials
//         );
//
//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }
// }
//
// public class LoginRequest {
//     public string Username { get; set; }
//     public string Password { get; set; }
// }
