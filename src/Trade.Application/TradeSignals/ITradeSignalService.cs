namespace Trade.Application.TradeSignals;

public interface ITradeSignalService
{
    Task<IReadOnlyList<TradeSignalDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<TradeSignalDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TradeSignalDto> CreateAsync(UpsertTradeSignalRequest request, CancellationToken cancellationToken);

    Task<TradeSignalDto> UpdateAsync(int id, UpsertTradeSignalRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
