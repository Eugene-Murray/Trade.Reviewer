using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Serilog;
using Trade.Application.Common.Services;
using Trade.Infrastructure;
using Trade.Infrastructure.Persistence;
using Trade.Mcp.Api.Features.Accounts;
using Trade.Mcp.Api.Features.TradeSignals;
using Trade.Mcp.Api.Features.Trades;
using Trade.Mcp.Api.Features.Watchlist;
using Trade.Mcp.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITradePerformanceCalculator, TradePerformanceCalculator>();
builder.Services.AddTradeInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TradeReviewerDbContext>("trade_dashboard");
builder.Services.AddCors(options =>
{
    options.AddPolicy("dashboard", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("dashboard");

await app.Services.InitialiseTradeInfrastructureAsync(app.Lifetime.ApplicationStopping);

app.MapGet("/", () => Results.Ok(new
{
    service = "Trade.Mcp.Api",
    database = "trade_dashboard",
    health = "/health",
}));

app.MapHealthChecks("/health");

var api = app.MapGroup("/api");
api.MapAccountEndpoints();
api.MapWatchlistEndpoints();
api.MapTradeSignalEndpoints();
api.MapTradeEndpoints();

app.Run();

public partial class Program;
