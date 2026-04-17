using Microsoft.EntityFrameworkCore;
using Trade.Domain.Entities;
using Trade.Domain.Enums;
using TradeEntity = Trade.Domain.Entities.Trade;
using Trade.Infrastructure.Persistence;

namespace Trade.Infrastructure.Seed;

internal static class TradeDashboardSeeder
{
    private static readonly IReadOnlyList<SeedStock> Stocks =
    [
        new("Apple (AAPL)", "Mega-cap hardware and services leader consolidating above long-term support.", 209.15m),
        new("Microsoft (MSFT)", "Cloud and AI platform leader with strong earnings momentum.", 431.40m),
        new("NVIDIA (NVDA)", "AI infrastructure leader tracking high-volume continuation setups.", 118.30m),
        new("Amazon (AMZN)", "Retail and AWS heavyweight holding a higher-low structure.", 188.75m),
        new("Meta Platforms (META)", "Digital advertising leader trading near relative-strength highs.", 541.10m),
        new("Alphabet Class A (GOOGL)", "Search and cloud platform watching for post-earnings follow-through.", 169.85m),
        new("Alphabet Class C (GOOG)", "Alternative Alphabet share class on the same breakout watch.", 171.05m),
        new("Tesla (TSLA)", "High-beta EV name with wide ranges and tactical swing setups.", 183.60m),
        new("Broadcom (AVGO)", "Semiconductor and infrastructure software compounder in strong trend.", 1385.25m),
        new("Costco (COST)", "Retail defensive leader trending above rising moving averages.", 838.90m),
        new("Netflix (NFLX)", "Streaming leader with momentum continuation potential.", 642.45m),
        new("Adobe (ADBE)", "Software platform name working through a basing pattern.", 471.80m),
        new("Cisco Systems (CSCO)", "Large-cap networking name holding a value breakout.", 61.95m),
        new("PepsiCo (PEP)", "Dividend compounder being monitored for mean-reversion support.", 171.10m),
        new("T-Mobile US (TMUS)", "Telecom leader showing resilient relative strength.", 166.45m),
        new("Qualcomm (QCOM)", "Chip designer tied to handset and AI edge demand.", 183.20m),
        new("Advanced Micro Devices (AMD)", "Semiconductor momentum name with volatile earnings reactions.", 163.55m),
        new("Intuit (INTU)", "Financial software leader with consistent cash-flow quality.", 647.15m),
        new("Intuitive Surgical (ISRG)", "Robotics healthcare leader trending steadily higher.", 425.75m),
        new("Amgen (AMGN)", "Biotech heavyweight with defensively stable price action.", 317.85m),
        new("Texas Instruments (TXN)", "Analog chip manufacturer approaching a multi-quarter base.", 194.65m),
        new("Booking Holdings (BKNG)", "Travel platform leader with strong free-cash-flow profile.", 3641.30m),
        new("Gilead Sciences (GILD)", "Large-cap biotech monitored for rotation into healthcare.", 74.25m),
        new("Honeywell (HON)", "Industrial tech conglomerate near range resistance.", 212.15m),
        new("Micron Technology (MU)", "Memory-cycle beneficiary trading with elevated momentum.", 127.95m),
        new("Applied Materials (AMAT)", "Semicap equipment name with improving order backdrop.", 221.70m),
        new("ARM Holdings (ARM)", "Recent large-cap listing tracking premium AI semiconductor flows.", 142.55m),
        new("KLA Corporation (KLAC)", "Inspection equipment leader continuing trend-channel behavior.", 758.20m),
        new("Analog Devices (ADI)", "High-quality analog semi name in a constructive base.", 223.45m),
        new("Lam Research (LRCX)", "Wafer fabrication supplier with cyclical upside leverage.", 927.85m),
        new("PDD Holdings (PDD)", "China e-commerce ADR monitored for event-driven reversals.", 142.65m),
        new("Regeneron Pharmaceuticals (REGN)", "Biotech compounder showing defensive accumulation.", 974.40m),
        new("Vertex Pharmaceuticals (VRTX)", "Healthcare growth leader holding above prior breakout.", 427.20m),
        new("Automatic Data Processing (ADP)", "Payroll and HR software name in steady uptrend.", 248.85m),
        new("Mondelez International (MDLZ)", "Consumer staple setup near support after pullback.", 72.50m),
        new("Starbucks (SBUX)", "Turnaround watchlist name with improving risk/reward.", 91.20m),
        new("Palo Alto Networks (PANW)", "Cybersecurity leader on momentum continuation watch.", 321.30m),
        new("Synopsys (SNPS)", "EDA software leader benefiting from chip design demand.", 602.35m),
        new("Cadence Design Systems (CDNS)", "EDA peer with strong recurring revenue profile.", 313.60m),
        new("Airbnb (ABNB)", "Travel platform swing setup around earnings gap levels.", 167.95m),
        new("CrowdStrike (CRWD)", "Security platform leader rebounding from sharp pullback.", 359.80m),
        new("Datadog (DDOG)", "Observability software name showing improving accumulation.", 132.70m),
        new("DexCom (DXCM)", "Medical devices leader in a long basing structure.", 141.90m),
        new("eBay (EBAY)", "Cash-generative marketplace name trading in a stable range.", 53.80m),
        new("Electronic Arts (EA)", "Game publisher with event-driven setup around launches.", 136.25m),
        new("Fiserv (FI)", "Payments software compounder near fresh highs.", 159.40m),
        new("Fortinet (FTNT)", "Cybersecurity platform monitored for trend continuation.", 69.10m),
        new("Marvell Technology (MRVL)", "AI networking beneficiary with higher beta exposure.", 74.35m),
        new("MercadoLibre (MELI)", "Latin America ecommerce leader with persistent strength.", 1778.60m),
        new("Old Dominion Freight Line (ODFL)", "Logistics leader watched for industrial breakout confirmation.", 207.55m),
    ];

    public static async Task SeedAsync(TradeReviewerDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Accounts.AnyAsync(cancellationToken))
        {
            return;
        }

        var accounts = new List<Account>
        {
            new() { AccountName = "Trading Account", AccountBalance = 150000.00m },
            new() { AccountName = "ISA Account", AccountBalance = 87500.00m },
            new() { AccountName = "SIPP Pension", AccountBalance = 260000.00m },
        };

        var watchlistItems = Stocks
            .Select((stock, index) => new WatchlistItem
            {
                StockName = stock.StockName,
                Notes = stock.Notes,
                DateAdded = new DateOnly(2026, 1, 2).AddDays(index),
            })
            .ToList();

        dbContext.Accounts.AddRange(accounts);
        dbContext.Watchlist.AddRange(watchlistItems);
        await dbContext.SaveChangesAsync(cancellationToken);

        var tradeSignals = Enumerable.Range(0, 8)
            .Select(index => new TradeSignal
            {
                StockName = Stocks[index].StockName,
                Direction = index % 2 == 0 ? TradeDirection.Long : TradeDirection.Short,
                SignalDate = new DateOnly(2026, 4, 1).AddDays(index - 8),
            })
            .ToList();

        var openPriceOffsets = new[] { -6.25m, -2.40m, 1.80m, 5.15m, 8.60m };
        var closedPriceOffsets = new[] { -8.10m, -5.55m, -2.15m, 1.65m, 4.90m, 7.25m, 10.85m };

        var openTrades = Enumerable.Range(0, 15)
            .Select(index =>
            {
                var stock = Stocks[index];
                var entryPrice = stock.BasePrice + (((index % 4) - 1.5m) * 2.35m);

                return new TradeEntity
                {
                    StockName = stock.StockName,
                    EntryPrice = decimal.Round(entryPrice, 2),
                    CurrentPrice = decimal.Round(entryPrice + openPriceOffsets[index % openPriceOffsets.Length], 2),
                    ClosePrice = null,
                    PositionSize = 10 + ((index % 5) * 5),
                    OpenDate = new DateOnly(2026, 2, 3).AddDays(index * 2),
                    CloseDate = null,
                    AccountId = accounts[index % accounts.Count].Id,
                };
            })
            .ToList();

        var closedTrades = Enumerable.Range(0, 150)
            .Select(index =>
            {
                var stock = Stocks[(index * 7) % Stocks.Count];
                var entryPrice = stock.BasePrice + (((index % 9) - 4m) * 1.15m);
                var closePrice = decimal.Round(entryPrice + closedPriceOffsets[index % closedPriceOffsets.Length], 2);
                var openDate = new DateOnly(2025, 5, 1).AddDays(index);

                return new TradeEntity
                {
                    StockName = stock.StockName,
                    EntryPrice = decimal.Round(entryPrice, 2),
                    CurrentPrice = closePrice,
                    ClosePrice = closePrice,
                    PositionSize = 5 + ((index % 8) * 5),
                    OpenDate = openDate,
                    CloseDate = openDate.AddDays(5 + (index % 18)),
                    AccountId = accounts[index % accounts.Count].Id,
                };
            })
            .ToList();

        dbContext.TradeSignals.AddRange(tradeSignals);
        dbContext.Trades.AddRange(openTrades);
        dbContext.Trades.AddRange(closedTrades);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record SeedStock(string StockName, string Notes, decimal BasePrice);
}
