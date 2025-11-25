using System.Text.Json;
using api;
using Microsoft.EntityFrameworkCore;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
var appOptions = builder.Services.AddAppOptions(builder.Configuration);
Console.WriteLine("the app options are: " + JsonSerializer.Serialize(appOptions));
builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});


// Add Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

// Build the app
var app = builder.Build();

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
        Playerid = Guid.NewGuid().ToString(),
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

app.Run();