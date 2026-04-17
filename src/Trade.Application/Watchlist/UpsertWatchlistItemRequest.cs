namespace Trade.Application.Watchlist;

public sealed record UpsertWatchlistItemRequest(
    string StockName,
    string Notes,
    DateOnly DateAdded);
