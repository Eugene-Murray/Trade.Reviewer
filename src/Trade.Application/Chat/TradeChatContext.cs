using Trade.Application.Accounts;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;

namespace Trade.Application.Chat;

public sealed record TradeChatContext(
    IReadOnlyList<AccountDto> Accounts,
    IReadOnlyList<TradeDto> Trades,
    IReadOnlyList<WatchlistItemDto> WatchlistItems,
    IReadOnlyList<TradeSignalDto> TradeSignals);
