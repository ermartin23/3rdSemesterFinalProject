using System.Text.Json;
using api;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger / OpenAPI
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<ITransactionService, TransactionService>();


var appOptions = builder.Services.AddAppOptions(builder.Configuration);
Console.WriteLine("the app options are: " + JsonSerializer.Serialize(appOptions));
//builder.Services.AddScoped<"Add IService and Service here (Alex uses ITodoService, TodoService) 1:37:21">();
builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});



// Build the app
var app = builder.Build();

app.UseExceptionHandler();

app.UseCors(config => config
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(x => true));

app.MapGet("/", (
    [FromServices]IOptionsMonitor<AppOptions> optionsMonitor,
    [FromServices]MyDbContext dbContext) =>
{
    
    var myPlayer = new Player()
    {
        Playerid = Guid.NewGuid(),
        Name = "Jeremy Morris",
        Phone = "+45 78 78 97 69",
        Email = "jeremy@morris.com",
        Active = true,
        Createdat = DateTime.Now
    };
    dbContext.Players.Add(myPlayer);
    dbContext.SaveChanges();
    var objects = dbContext.Players.ToList();
    return objects;
});

// Enable Swagger UI in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseOpenApi();
app.UseSwaggerUi();
await app.GenerateApiClientsFromOpenApi("/../../client/src/core/generated-client.ts");

app.Run();