using NovaLab.Server.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
    options.UseSqlServer();
});

WebApplication app = builder.Build();

app.Run();
