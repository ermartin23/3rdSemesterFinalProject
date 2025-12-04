using System.Text.Json;
using api;
using api.Features.Games;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ❌ COMENTAR ESTO SI TU BRANCH NO TIENE PLAYER SERVICE
// builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IGameService, GameService>();

var appOptions = builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(config => config
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true));

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
