using Microsoft.EntityFrameworkCore;
using Trade.Application.Accounts;
using Trade.Application.Common.Exceptions;
using Trade.Infrastructure.Persistence;

namespace Trade.Infrastructure.Accounts;

internal sealed class AccountService(TradeReviewerDbContext dbContext) : IAccountService
{
    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Accounts
            .AsNoTracking()
            .OrderBy(account => account.AccountName)
            .Select(account => new AccountDto(
                account.Id,
                account.AccountName,
                account.AccountBalance,
                account.Trades.Count(trade => trade.CloseDate == null),
                account.Trades
                    .Where(trade => trade.CloseDate == null)
                    .Sum(trade => (decimal?)(trade.CurrentPrice * trade.PositionSize)) ?? 0m))
            .ToListAsync(cancellationToken);

    public async Task<AccountDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new AccountDto(
                item.Id,
                item.AccountName,
                item.AccountBalance,
                item.Trades.Count(trade => trade.CloseDate == null),
                item.Trades
                    .Where(trade => trade.CloseDate == null)
                    .Sum(trade => (decimal?)(trade.CurrentPrice * trade.PositionSize)) ?? 0m))
            .SingleOrDefaultAsync(cancellationToken);

        return account ?? throw new ResourceNotFoundException($"Account {id} was not found.");
    }

    public async Task<AccountDto> CreateAsync(UpsertAccountRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var duplicateNameExists = await dbContext.Accounts
            .AnyAsync(account => account.AccountName == request.AccountName, cancellationToken);

        if (duplicateNameExists)
        {
            throw new ValidationException($"An account named '{request.AccountName}' already exists.");
        }

        var account = new Trade.Domain.Entities.Account
        {
            AccountName = request.AccountName.Trim(),
            AccountBalance = request.AccountBalance,
        };

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(account.Id, cancellationToken);
    }

    public async Task<AccountDto> UpdateAsync(int id, UpsertAccountRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var account = await dbContext.Accounts.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Account {id} was not found.");

        var duplicateNameExists = await dbContext.Accounts
            .AnyAsync(item => item.Id != id && item.AccountName == request.AccountName, cancellationToken);

        if (duplicateNameExists)
        {
            throw new ValidationException($"An account named '{request.AccountName}' already exists.");
        }

        account.AccountName = request.AccountName.Trim();
        account.AccountBalance = request.AccountBalance;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .Include(item => item.Trades)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Account {id} was not found.");

        if (account.Trades.Count > 0)
        {
            throw new ValidationException("Accounts with trades cannot be deleted.");
        }

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(UpsertAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AccountName))
        {
            throw new ValidationException("Account name is required.");
        }

        if (request.AccountBalance < 0)
        {
            throw new ValidationException("Account balance cannot be negative.");
        }
    }
}
