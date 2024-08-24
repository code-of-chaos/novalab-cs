// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using Microsoft.EntityFrameworkCore;
using NovaLab.Database;

namespace NovaLab.API;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class BaseController(IDbContextFactory<NovaLabDbContext> contextFactory)
    : AbstractBaseController<NovaLabDbContext>(contextFactory);
