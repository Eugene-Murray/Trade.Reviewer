namespace Trade.Application.Accounts;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<AccountDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<AccountDto> CreateAsync(UpsertAccountRequest request, CancellationToken cancellationToken);

    Task<AccountDto> UpdateAsync(int id, UpsertAccountRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
