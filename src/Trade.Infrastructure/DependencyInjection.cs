using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trade.Application.Accounts;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;
using Trade.Infrastructure.Accounts;
using Trade.Infrastructure.Persistence;
using Trade.Infrastructure.Seed;
using Trade.Infrastructure.TradeSignals;
using Trade.Infrastructure.Trades;
using Trade.Infrastructure.Watchlist;

namespace Trade.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTradeInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TradeDashboard")
            ?? "Data Source=trade_dashboard.db";

        services.AddDbContext<TradeReviewerDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IWatchlistService, WatchlistService>();
        services.AddScoped<ITradeSignalService, TradeSignalService>();
        services.AddScoped<ITradeService, TradeService>();

        return services;
    }

    public static async Task InitialiseTradeInfrastructureAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradeReviewerDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        await TradeDashboardSeeder.SeedAsync(dbContext, cancellationToken);
    }
}
