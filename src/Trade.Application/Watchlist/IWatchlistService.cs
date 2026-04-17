namespace Trade.Application.Watchlist;

public interface IWatchlistService
{
    Task<IReadOnlyList<WatchlistItemDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<WatchlistItemDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<WatchlistItemDto> CreateAsync(UpsertWatchlistItemRequest request, CancellationToken cancellationToken);

    Task<WatchlistItemDto> UpdateAsync(int id, UpsertWatchlistItemRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
