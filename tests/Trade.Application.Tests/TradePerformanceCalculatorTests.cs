using Trade.Application.Common.Services;
using TradeEntity = Trade.Domain.Entities.Trade;

namespace Trade.Application.Tests;

public sealed class TradePerformanceCalculatorTests
{
    private readonly TradePerformanceCalculator _calculator = new();

    [Fact]
    public void CalculateProfitLossForOpenTradeUsesCurrentPrice()
    {
        var trade = new TradeEntity
        {
            EntryPrice = 50m,
            CurrentPrice = 55.5m,
            PositionSize = 20m,
        };

        var result = _calculator.CalculateProfitLoss(trade);

        Assert.Equal(110m, result);
    }

    [Fact]
    public void CalculateProfitLossForClosedTradeUsesClosePrice()
    {
        var trade = new TradeEntity
        {
            EntryPrice = 80m,
            CurrentPrice = 95m,
            ClosePrice = 76m,
            PositionSize = 15m,
            CloseDate = new DateOnly(2026, 4, 17),
        };

        var result = _calculator.CalculateProfitLoss(trade);

        Assert.Equal(-60m, result);
    }
}
