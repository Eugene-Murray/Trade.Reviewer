namespace Trade.Application.Trades;

public sealed record UpsertTradeRequest(
    string StockName,
    decimal EntryPrice,
    decimal CurrentPrice,
    decimal? ClosePrice,
    decimal PositionSize,
    DateOnly OpenDate,
    DateOnly? CloseDate,
    int AccountId);
