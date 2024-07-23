// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data;

namespace NovaLab.API;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class BaseController(IDbContextFactory<NovaLabDbContext> contextFactory)
    : AbstractBaseController<NovaLabDbContext>(contextFactory);
