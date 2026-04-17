namespace Trade.Domain.Entities;

public sealed class WatchlistItem
{
    public int Id { get; set; }

    public string StockName { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public DateOnly DateAdded { get; set; }

    public ICollection<TradeSignal> TradeSignals { get; set; } = [];

    public ICollection<Trade> Trades { get; set; } = [];
}
