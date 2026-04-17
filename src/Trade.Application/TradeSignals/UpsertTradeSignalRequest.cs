using Trade.Domain.Enums;

namespace Trade.Application.TradeSignals;

public sealed record UpsertTradeSignalRequest(
    string StockName,
    TradeDirection Direction,
    DateOnly SignalDate);
