using Microsoft.EntityFrameworkCore;
using Trade.Application.Common.Exceptions;
using Trade.Application.Common.Services;
using Trade.Application.Trades;
using Trade.Infrastructure.Persistence;

namespace Trade.Infrastructure.Trades;

internal sealed class TradeService(
    TradeReviewerDbContext dbContext,
    ITradePerformanceCalculator performanceCalculator) : ITradeService
{
    public async Task<IReadOnlyList<TradeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var trades = await dbContext.Trades
            .AsNoTracking()
            .Include(trade => trade.Account)
            .OrderByDescending(trade => trade.OpenDate)
            .ThenBy(trade => trade.StockName)
            .ToListAsync(cancellationToken);

        return trades
            .Select(Map)
            .ToList();
    }

    public async Task<TradeDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var trade = await dbContext.Trades
            .AsNoTracking()
            .Include(item => item.Account)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Trade {id} was not found.");

        return Map(trade);
    }

    public async Task<TradeDto> CreateAsync(UpsertTradeRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureDependenciesExistAsync(request.StockName, request.AccountId, cancellationToken);

        var trade = new Trade.Domain.Entities.Trade
        {
            StockName = request.StockName.Trim(),
            EntryPrice = request.EntryPrice,
            CurrentPrice = request.CurrentPrice,
            ClosePrice = request.ClosePrice,
            PositionSize = request.PositionSize,
            OpenDate = request.OpenDate,
            CloseDate = request.CloseDate,
            AccountId = request.AccountId,
        };

        dbContext.Trades.Add(trade);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(trade.Id, cancellationToken);
    }

    public async Task<TradeDto> UpdateAsync(int id, UpsertTradeRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureDependenciesExistAsync(request.StockName, request.AccountId, cancellationToken);

        var trade = await dbContext.Trades.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Trade {id} was not found.");

        trade.StockName = request.StockName.Trim();
        trade.EntryPrice = request.EntryPrice;
        trade.CurrentPrice = request.CurrentPrice;
        trade.ClosePrice = request.ClosePrice;
        trade.PositionSize = request.PositionSize;
        trade.OpenDate = request.OpenDate;
        trade.CloseDate = request.CloseDate;
        trade.AccountId = request.AccountId;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var trade = await dbContext.Trades.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Trade {id} was not found.");

        dbContext.Trades.Remove(trade);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDependenciesExistAsync(string stockName, int accountId, CancellationToken cancellationToken)
    {
        var normalizedStockName = stockName.Trim();
        if (!await dbContext.Watchlist.AnyAsync(item => item.StockName == normalizedStockName, cancellationToken))
        {
            throw new ValidationException($"'{normalizedStockName}' must exist on the watchlist before trades can be created.");
        }

        if (!await dbContext.Accounts.AnyAsync(item => item.Id == accountId, cancellationToken))
        {
            throw new ValidationException($"Account {accountId} does not exist.");
        }
    }

    private static void Validate(UpsertTradeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StockName))
        {
            throw new ValidationException("Stock name is required.");
        }

        if (request.EntryPrice <= 0 || request.CurrentPrice <= 0)
        {
            throw new ValidationException("Entry price and current price must be positive.");
        }

        if (request.PositionSize <= 0)
        {
            throw new ValidationException("Position size must be positive.");
        }

        if (request.CloseDate.HasValue && request.ClosePrice is null)
        {
            throw new ValidationException("Closed trades must include a close price.");
        }

        if (!request.CloseDate.HasValue && request.ClosePrice.HasValue)
        {
            throw new ValidationException("Open trades cannot include a close price.");
        }
    }

    private TradeDto Map(Trade.Domain.Entities.Trade trade) =>
        new(
            trade.Id,
            trade.StockName,
            trade.EntryPrice,
            trade.CurrentPrice,
            trade.ClosePrice,
            trade.PositionSize,
            trade.OpenDate,
            trade.CloseDate,
            trade.AccountId,
            trade.Account.AccountName,
            performanceCalculator.CalculateProfitLoss(trade));
}
