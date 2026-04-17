using System.ComponentModel.DataAnnotations;
using Trade.Application.TradeSignals;
using Trade.Domain.Enums;

namespace Trade.Dashboard.Client.Models;

public sealed class TradeSignalFormModel
{
    [Required]
    [StringLength(120)]
    public string StockName { get; set; } = string.Empty;

    public TradeDirection Direction { get; set; } = TradeDirection.Long;

    public DateOnly SignalDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public UpsertTradeSignalRequest ToRequest() => new(StockName, Direction, SignalDate);

    public static TradeSignalFormModel FromDto(TradeSignalDto dto) => new()
    {
        StockName = dto.StockName,
        Direction = dto.Direction,
        SignalDate = dto.SignalDate,
    };
}
