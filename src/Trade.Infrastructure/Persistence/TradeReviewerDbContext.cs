using Microsoft.EntityFrameworkCore;
using Trade.Domain.Entities;
using TradeEntity = Trade.Domain.Entities.Trade;
using Trade.Infrastructure.Persistence.Configurations;

namespace Trade.Infrastructure.Persistence;

public sealed class TradeReviewerDbContext(DbContextOptions<TradeReviewerDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<TradeEntity> Trades => Set<TradeEntity>();

    public DbSet<TradeSignal> TradeSignals => Set<TradeSignal>();

    public DbSet<WatchlistItem> Watchlist => Set<WatchlistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new WatchlistItemConfiguration());
        modelBuilder.ApplyConfiguration(new TradeSignalConfiguration());
        modelBuilder.ApplyConfiguration(new TradeConfiguration());
    }
}
