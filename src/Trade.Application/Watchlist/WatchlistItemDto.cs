namespace Trade.Application.Watchlist;

public sealed record WatchlistItemDto(
    int Id,
    string StockName,
    string Notes,
    DateOnly DateAdded,
    int SignalCount,
    int OpenTradeCount);
