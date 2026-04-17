using Trade.Domain.Enums;

namespace Trade.Domain.Entities;

public sealed class TradeSignal
{
    public int Id { get; set; }

    public string StockName { get; set; } = string.Empty;

    public TradeDirection Direction { get; set; }

    public DateOnly SignalDate { get; set; }

    public WatchlistItem? WatchlistItem { get; set; }
}
