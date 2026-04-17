using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Trade.Infrastructure.Persistence;

public sealed class TradeReviewerDbContextFactory : IDesignTimeDbContextFactory<TradeReviewerDbContext>
{
    public TradeReviewerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TradeReviewerDbContext>();
        optionsBuilder.UseSqlite("Data Source=trade_dashboard.db");

        return new TradeReviewerDbContext(optionsBuilder.Options);
    }
}
