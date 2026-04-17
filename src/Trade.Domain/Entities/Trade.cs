namespace Trade.Domain.Entities;

public sealed class Trade
{
    public int Id { get; set; }

    public string StockName { get; set; } = string.Empty;

    public decimal EntryPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal? ClosePrice { get; set; }

    public decimal PositionSize { get; set; }

    public DateOnly OpenDate { get; set; }

    public DateOnly? CloseDate { get; set; }

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public WatchlistItem? WatchlistItem { get; set; }

    public bool IsClosed => CloseDate.HasValue;

    public decimal ProfitLoss =>
        (((CloseDate.HasValue ? ClosePrice : null) ?? CurrentPrice) - EntryPrice) * PositionSize;
}
