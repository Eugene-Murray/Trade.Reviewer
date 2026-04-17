using Trade.Application.Accounts;
using Trade.Application.Chat;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;
using Trade.Domain.Enums;

namespace Trade.Application.Tests;

public sealed class TradeQuestionAnswerBuilderTests
{
    private readonly TradeQuestionAnswerBuilder _builder = new();

    [Fact]
    public void BuildAnswerForAccountQuestionIncludesMatchedAccount()
    {
        var context = CreateContext();

        var result = _builder.BuildAnswer(
            "How is the Trading Account doing?",
            context,
            [ "get_accounts", "get_trades" ]);

        Assert.Contains("Trading Account", result.ReferencedAccounts);
        Assert.Contains("## Accounts", result.Answer, StringComparison.Ordinal);
        Assert.Contains("balance", result.Answer, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(2, result.UsedTools.Count);
    }

    [Fact]
    public void BuildAnswerForStockQuestionIncludesWatchlistAndSignals()
    {
        var context = CreateContext();

        var result = _builder.BuildAnswer(
            "Is AAPL on the watchlist and what signals do I have for it?",
            context,
            [ "get_watchlist", "get_trade_signals" ]);

        Assert.Contains("Apple (AAPL)", result.ReferencedStocks);
        Assert.Contains("## Watchlist", result.Answer, StringComparison.Ordinal);
        Assert.Contains("## Trade signals", result.Answer, StringComparison.Ordinal);
        Assert.Contains("Long", result.Answer, StringComparison.Ordinal);
    }

    private static TradeChatContext CreateContext() =>
        new(
            [
                new AccountDto(1, "Trading Account", 150000m, 2, 25000m),
                new AccountDto(2, "ISA Account", 87500m, 0, 0m),
            ],
            [
                new TradeDto(1, "Apple (AAPL)", 180m, 190m, null, 10m, new DateOnly(2026, 4, 1), null, 1, "Trading Account", 100m),
                new TradeDto(2, "Microsoft (MSFT)", 420m, 418m, null, 5m, new DateOnly(2026, 4, 3), null, 1, "Trading Account", -10m),
            ],
            [
                new WatchlistItemDto(1, "Apple (AAPL)", "Watching for continuation.", new DateOnly(2026, 1, 2), 1, 1),
                new WatchlistItemDto(2, "Microsoft (MSFT)", "Cloud leader on strength.", new DateOnly(2026, 1, 3), 0, 1),
            ],
            [
                new TradeSignalDto(1, "Apple (AAPL)", TradeDirection.Long, new DateOnly(2026, 4, 15)),
            ]);
}
