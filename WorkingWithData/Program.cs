using Microsoft.EntityFrameworkCore;
using WorkingWithData.Contexts;
using WorkingWithData.Models;
using WorkingWithData.Platform;

var builder = WebApplication.CreateBuilder(args);

                //AddStackExchangeRedisCache -> Usado para o Redis
builder.Services.AddDistributedSqlServerCache(opts =>
{
    opts.ConnectionString = builder.Configuration.GetConnectionString("CalcConnection");

    opts.SchemaName = "dbo";
    opts.TableName = "DataCache";
});


builder.Services.AddResponseCaching(); // Salvando a RESPONSE no cache

builder.Services.AddDbContext<CalculationContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("CacheConnection"));
    opts.EnableSensitiveDataLogging(true);
});

builder.Services.AddTransient<SeedData>();

var app = builder.Build();

app.UseResponseCaching(); // Salvando a RESPONSE no cache

app.MapEndpoint<SumEndpoint>("/sum/{count:int=10000000}");

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello");
});

bool cmdLineInit = (app.Configuration["INITDB"] ?? "false") == "true";

if(app.Environment.IsDevelopment() || cmdLineInit)
{
   using(var scope = app.Services.CreateScope())
    {
        var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();

        seedData.SeedDatabase();
    }
}

if (!cmdLineInit)
{
    app.Run();
}

