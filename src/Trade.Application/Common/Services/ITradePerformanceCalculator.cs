using TradeEntity = Trade.Domain.Entities.Trade;

namespace Trade.Application.Common.Services;

public interface ITradePerformanceCalculator
{
    decimal CalculateProfitLoss(TradeEntity trade);
}
