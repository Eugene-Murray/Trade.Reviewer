namespace Trade.Application.Trades;

public sealed record TradeDto(
    int Id,
    string StockName,
    decimal EntryPrice,
    decimal CurrentPrice,
    decimal? ClosePrice,
    decimal PositionSize,
    DateOnly OpenDate,
    DateOnly? CloseDate,
    int AccountId,
    string AccountName,
    decimal ProfitLoss);
