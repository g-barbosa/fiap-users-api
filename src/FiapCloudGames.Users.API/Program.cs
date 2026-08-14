using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Services;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.Services;
using FiapCloudGames.Users.Infrastructure.Data.Persistence;
using FiapCloudGames.Users.Infrastructure.Data.Persistence.Repositories;
using FiapCloudGames.Users.Infrastructure.Security;
using FiapCloudGames.Users.Infrastructure.Security.settings;
using FiapCloudGames.Users.Infrastructure.Services;
using FiapCloudGames.Users.Infrastructure.Caching;
using FiapCloudGames.Users.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using FiapCloudGames.Users.Application.Usuarios.Interfaces.Messaging;
using FiapCloudGames.Users.Infrastructure.Messaging.Publishers;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("Comum", policy => policy.RequireRole("Comum"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "FIAP Cloud Games API - Tech Challenge Grupo 6",
        Description = "API para o desafio técnico do grupo 6 da FIAP Cloud Games",
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq";
var rabbitMqPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
var rabbitMqUri = new Uri($"amqp://admin:rabbitmq123@{rabbitMqHost}:{rabbitMqPort}/");

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "", name: "SqlServer");

builder.Services.AddDbContext<FiapCloudGamesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
}, ServiceLifetime.Scoped);

var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "fiap-users:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.Configure<JwtConfigs>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<ICorrelationIdService, CorrelationIdService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioCache, RedisUsuarioCache>();
builder.Services.AddScoped<IUsuarioDomainService, UsuarioDomainService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioEventPublisher, RabbitMqUsuarioEventPublisher>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FiapCloudGamesDbContext>();
    dbContext.Database.Migrate();
}

app.UseCorrelationId();
app.UseErrorHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.MapMetrics();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }