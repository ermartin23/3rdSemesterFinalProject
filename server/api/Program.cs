using System.Text.Json;
using api;
using api.Features.Boards;
using api.Features.Games;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using api.Features.Players;
using api.Features.Admins;
using api.Features.Auth;
using api.Features.RepeatingBoards;
using api.Helpers.Time;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", p => p
        .WithOrigins(
            "http://localhost:5173", 
            "https://deadpigeons-aged-voice-1352.fly.dev" 
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IRepeatingBoardService, RepeatingBoardService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var appOptions = builder.Services.AddAppOptions(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = appOptions.JwtIssuer,

            ValidateAudience = true,
            ValidAudience = appOptions.JwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appOptions.JwtSecret)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

Console.WriteLine("DbConnectionString empty? " + string.IsNullOrWhiteSpace(appOptions.DbConnectionString));
Console.WriteLine("JwtSecret empty? " + string.IsNullOrWhiteSpace(appOptions.JwtSecret));

builder.Services.AddAuthorization();

Console.WriteLine($"AppOptions loaded. Issuer={appOptions.JwtIssuer}, Audience={appOptions.JwtAudience}");

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
}

app.UseExceptionHandler();

app.UseRouting();
app.UseCors("Client");

app.UseAuthentication();
app.UseAuthorization();

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