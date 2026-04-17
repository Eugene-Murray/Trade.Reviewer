using Microsoft.EntityFrameworkCore;
using Trade.Application.Common.Exceptions;
using Trade.Application.TradeSignals;
using Trade.Infrastructure.Persistence;

namespace Trade.Infrastructure.TradeSignals;

internal sealed class TradeSignalService(TradeReviewerDbContext dbContext) : ITradeSignalService
{
    public async Task<IReadOnlyList<TradeSignalDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.TradeSignals
            .AsNoTracking()
            .OrderByDescending(signal => signal.SignalDate)
            .ThenBy(signal => signal.StockName)
            .Select(signal => new TradeSignalDto(
                signal.Id,
                signal.StockName,
                signal.Direction,
                signal.SignalDate))
            .ToListAsync(cancellationToken);

    public async Task<TradeSignalDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var signal = await dbContext.TradeSignals
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new TradeSignalDto(
                item.Id,
                item.StockName,
                item.Direction,
                item.SignalDate))
            .SingleOrDefaultAsync(cancellationToken);

        return signal ?? throw new ResourceNotFoundException($"Trade signal {id} was not found.");
    }

    public async Task<TradeSignalDto> CreateAsync(UpsertTradeSignalRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureWatchlistItemExistsAsync(request.StockName, cancellationToken);

        var signal = new Trade.Domain.Entities.TradeSignal
        {
            StockName = request.StockName.Trim(),
            Direction = request.Direction,
            SignalDate = request.SignalDate,
        };

        dbContext.TradeSignals.Add(signal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(signal.Id, cancellationToken);
    }

    public async Task<TradeSignalDto> UpdateAsync(int id, UpsertTradeSignalRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureWatchlistItemExistsAsync(request.StockName, cancellationToken);

        var signal = await dbContext.TradeSignals.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Trade signal {id} was not found.");

        signal.StockName = request.StockName.Trim();
        signal.Direction = request.Direction;
        signal.SignalDate = request.SignalDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var signal = await dbContext.TradeSignals.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Trade signal {id} was not found.");

        dbContext.TradeSignals.Remove(signal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureWatchlistItemExistsAsync(string stockName, CancellationToken cancellationToken)
    {
        var normalizedStockName = stockName.Trim();
        if (!await dbContext.Watchlist.AnyAsync(item => item.StockName == normalizedStockName, cancellationToken))
        {
            throw new ValidationException($"'{normalizedStockName}' must exist on the watchlist before signals can be created.");
        }
    }

    private static void Validate(UpsertTradeSignalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StockName))
        {
            throw new ValidationException("Stock name is required.");
        }
    }
}
