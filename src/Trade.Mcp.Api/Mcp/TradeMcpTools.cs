using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;
using Trade.Application.Accounts;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;

namespace Trade.Mcp.Api.Mcp;

[McpServerToolType]
internal sealed class TradeMcpTools(
    IAccountService accountService,
    ITradeService tradeService,
    IWatchlistService watchlistService,
    ITradeSignalService tradeSignalService)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [McpServerTool(Name = McpToolNames.GetAccounts, ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Returns all trading accounts as JSON, including balances, open trade counts, and open exposure.")]
    public async Task<string> GetAccountsAsync(CancellationToken cancellationToken) =>
        JsonSerializer.Serialize(await accountService.GetAllAsync(cancellationToken), SerializerOptions);

    [McpServerTool(Name = McpToolNames.GetTrades, ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Returns all trades as JSON, including account names, prices, dates, and profit or loss values.")]
    public async Task<string> GetTradesAsync(CancellationToken cancellationToken) =>
        JsonSerializer.Serialize(await tradeService.GetAllAsync(cancellationToken), SerializerOptions);

    [McpServerTool(Name = McpToolNames.GetWatchlist, ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Returns all watchlist items as JSON, including notes, dates added, signal counts, and open trade counts.")]
    public async Task<string> GetWatchlistAsync(CancellationToken cancellationToken) =>
        JsonSerializer.Serialize(await watchlistService.GetAllAsync(cancellationToken), SerializerOptions);

    [McpServerTool(Name = McpToolNames.GetTradeSignals, ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Returns all trade signals as JSON, including stock names, long or short direction, and signal dates.")]
    public async Task<string> GetTradeSignalsAsync(CancellationToken cancellationToken) =>
        JsonSerializer.Serialize(await tradeSignalService.GetAllAsync(cancellationToken), SerializerOptions);
}
