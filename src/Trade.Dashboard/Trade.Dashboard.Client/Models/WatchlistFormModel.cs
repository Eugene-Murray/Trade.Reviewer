using System.ComponentModel.DataAnnotations;
using Trade.Application.Watchlist;

namespace Trade.Dashboard.Client.Models;

public sealed class WatchlistFormModel
{
    [Required]
    [StringLength(120)]
    public string StockName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public DateOnly DateAdded { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public UpsertWatchlistItemRequest ToRequest() => new(StockName, Notes, DateAdded);

    public static WatchlistFormModel FromDto(WatchlistItemDto dto) => new()
    {
        StockName = dto.StockName,
        Notes = dto.Notes,
        DateAdded = dto.DateAdded,
    };
}
