// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NovaLab.Servers.API.Controllers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthTest {
    
    [HttpGet]
    [Authorize]
    public IActionResult GetSecret() {
        return new OkResult();
    }
}
