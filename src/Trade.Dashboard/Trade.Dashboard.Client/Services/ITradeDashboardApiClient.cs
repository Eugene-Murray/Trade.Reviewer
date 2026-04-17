using Trade.Application.Accounts;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;

namespace Trade.Dashboard.Client.Services;

public interface ITradeDashboardApiClient
{
    Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<AccountDto> CreateAccountAsync(UpsertAccountRequest request, CancellationToken cancellationToken = default);

    Task<AccountDto> UpdateAccountAsync(int id, UpsertAccountRequest request, CancellationToken cancellationToken = default);

    Task DeleteAccountAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WatchlistItemDto>> GetWatchlistAsync(CancellationToken cancellationToken = default);

    Task<WatchlistItemDto> CreateWatchlistItemAsync(UpsertWatchlistItemRequest request, CancellationToken cancellationToken = default);

    Task<WatchlistItemDto> UpdateWatchlistItemAsync(int id, UpsertWatchlistItemRequest request, CancellationToken cancellationToken = default);

    Task DeleteWatchlistItemAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TradeSignalDto>> GetTradeSignalsAsync(CancellationToken cancellationToken = default);

    Task<TradeSignalDto> CreateTradeSignalAsync(UpsertTradeSignalRequest request, CancellationToken cancellationToken = default);

    Task<TradeSignalDto> UpdateTradeSignalAsync(int id, UpsertTradeSignalRequest request, CancellationToken cancellationToken = default);

    Task DeleteTradeSignalAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TradeDto>> GetTradesAsync(CancellationToken cancellationToken = default);

    Task<TradeDto> CreateTradeAsync(UpsertTradeRequest request, CancellationToken cancellationToken = default);

    Task<TradeDto> UpdateTradeAsync(int id, UpsertTradeRequest request, CancellationToken cancellationToken = default);

    Task DeleteTradeAsync(int id, CancellationToken cancellationToken = default);
}
