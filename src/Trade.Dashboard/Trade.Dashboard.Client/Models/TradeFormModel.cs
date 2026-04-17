using System.ComponentModel.DataAnnotations;
using Trade.Application.Trades;

namespace Trade.Dashboard.Client.Models;

public sealed class TradeFormModel
{
    [Required]
    [StringLength(120)]
    public string StockName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal EntryPrice { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal CurrentPrice { get; set; }

    public decimal? ClosePrice { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal PositionSize { get; set; }

    public DateOnly OpenDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public DateOnly? CloseDate { get; set; }

    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }

    public bool IsClosed { get; set; }

    public UpsertTradeRequest ToRequest() => new(
        StockName,
        EntryPrice,
        CurrentPrice,
        IsClosed ? ClosePrice : null,
        PositionSize,
        OpenDate,
        IsClosed ? CloseDate : null,
        AccountId);

    public static TradeFormModel FromDto(TradeDto dto) => new()
    {
        StockName = dto.StockName,
        EntryPrice = dto.EntryPrice,
        CurrentPrice = dto.CurrentPrice,
        ClosePrice = dto.ClosePrice,
        PositionSize = dto.PositionSize,
        OpenDate = dto.OpenDate,
        CloseDate = dto.CloseDate,
        AccountId = dto.AccountId,
        IsClosed = dto.CloseDate.HasValue,
    };
}
