using Microsoft.EntityFrameworkCore;
using Trade.Application.Common.Exceptions;
using Trade.Application.Watchlist;
using Trade.Infrastructure.Persistence;

namespace Trade.Infrastructure.Watchlist;

internal sealed class WatchlistService(TradeReviewerDbContext dbContext) : IWatchlistService
{
    public async Task<IReadOnlyList<WatchlistItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Watchlist
            .AsNoTracking()
            .OrderBy(item => item.StockName)
            .Select(item => new WatchlistItemDto(
                item.Id,
                item.StockName,
                item.Notes,
                item.DateAdded,
                item.TradeSignals.Count,
                item.Trades.Count(trade => trade.CloseDate == null)))
            .ToListAsync(cancellationToken);

    public async Task<WatchlistItemDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await dbContext.Watchlist
            .AsNoTracking()
            .Where(entry => entry.Id == id)
            .Select(entry => new WatchlistItemDto(
                entry.Id,
                entry.StockName,
                entry.Notes,
                entry.DateAdded,
                entry.TradeSignals.Count,
                entry.Trades.Count(trade => trade.CloseDate == null)))
            .SingleOrDefaultAsync(cancellationToken);

        return item ?? throw new ResourceNotFoundException($"Watchlist item {id} was not found.");
    }

    public async Task<WatchlistItemDto> CreateAsync(UpsertWatchlistItemRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var normalizedStockName = request.StockName.Trim();
        if (await dbContext.Watchlist.AnyAsync(item => item.StockName == normalizedStockName, cancellationToken))
        {
            throw new ValidationException($"'{normalizedStockName}' is already on the watchlist.");
        }

        var item = new Trade.Domain.Entities.WatchlistItem
        {
            StockName = normalizedStockName,
            Notes = request.Notes.Trim(),
            DateAdded = request.DateAdded,
        };

        dbContext.Watchlist.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(item.Id, cancellationToken);
    }

    public async Task<WatchlistItemDto> UpdateAsync(int id, UpsertWatchlistItemRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var item = await dbContext.Watchlist.SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Watchlist item {id} was not found.");

        var normalizedStockName = request.StockName.Trim();
        if (await dbContext.Watchlist.AnyAsync(entry => entry.Id != id && entry.StockName == normalizedStockName, cancellationToken))
        {
            throw new ValidationException($"'{normalizedStockName}' is already on the watchlist.");
        }

        item.StockName = normalizedStockName;
        item.Notes = request.Notes.Trim();
        item.DateAdded = request.DateAdded;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var item = await dbContext.Watchlist
            .Include(entry => entry.TradeSignals)
            .Include(entry => entry.Trades)
            .SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Watchlist item {id} was not found.");

        if (item.TradeSignals.Count > 0 || item.Trades.Count > 0)
        {
            throw new ValidationException("Watchlist items with linked signals or trades cannot be deleted.");
        }

        dbContext.Watchlist.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(UpsertWatchlistItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StockName))
        {
            throw new ValidationException("Stock name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Notes))
        {
            throw new ValidationException("Notes are required.");
        }
    }
}
