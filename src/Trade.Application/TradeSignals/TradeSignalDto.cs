using Trade.Domain.Enums;

namespace Trade.Application.TradeSignals;

public sealed record TradeSignalDto(
    int Id,
    string StockName,
    TradeDirection Direction,
    DateOnly SignalDate);
