namespace Trade.Application.Trades;

public interface ITradeService
{
    Task<IReadOnlyList<TradeDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<TradeDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TradeDto> CreateAsync(UpsertTradeRequest request, CancellationToken cancellationToken);

    Task<TradeDto> UpdateAsync(int id, UpsertTradeRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
