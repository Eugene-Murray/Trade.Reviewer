using TradeEntity = Trade.Domain.Entities.Trade;

namespace Trade.Domain.Tests;

public sealed class TradeTests
{
    [Fact]
    public void ProfitLossForOpenTradeUsesCurrentPrice()
    {
        var trade = new TradeEntity
        {
            EntryPrice = 100m,
            CurrentPrice = 112m,
            PositionSize = 10m,
        };

        Assert.Equal(120m, trade.ProfitLoss);
        Assert.False(trade.IsClosed);
    }

    [Fact]
    public void ProfitLossForClosedTradeUsesClosePrice()
    {
        var trade = new TradeEntity
        {
            EntryPrice = 150m,
            CurrentPrice = 180m,
            ClosePrice = 140m,
            PositionSize = 4m,
            CloseDate = new DateOnly(2026, 4, 17),
        };

        Assert.Equal(-40m, trade.ProfitLoss);
        Assert.True(trade.IsClosed);
    }
}
