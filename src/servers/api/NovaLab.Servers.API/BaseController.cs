// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Database;

namespace NovaLab.Servers.API;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class BaseController(IDbContextFactory<NovaLabDbContext> contextFactory)
    : AbstractBaseController<NovaLabDbContext>(contextFactory);
