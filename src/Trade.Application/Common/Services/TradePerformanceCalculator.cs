using TradeEntity = Trade.Domain.Entities.Trade;

namespace Trade.Application.Common.Services;

public sealed class TradePerformanceCalculator : ITradePerformanceCalculator
{
    public decimal CalculateProfitLoss(TradeEntity trade)
    {
        ArgumentNullException.ThrowIfNull(trade);

        var exitPrice = trade.CloseDate.HasValue ? trade.ClosePrice ?? trade.CurrentPrice : trade.CurrentPrice;
        return (exitPrice - trade.EntryPrice) * trade.PositionSize;
    }
}
