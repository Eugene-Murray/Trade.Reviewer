using System.Text;
using System.Globalization;
using Trade.Application.Accounts;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;

namespace Trade.Application.Chat;

public sealed class TradeQuestionAnswerBuilder : ITradeQuestionAnswerBuilder
{
    public ChatAnswerDto BuildAnswer(string question, TradeChatContext context, IReadOnlyList<string> usedTools)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(question);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(usedTools);

        var matchedAccounts = context.Accounts
            .Where(account => ContainsPhrase(question, account.AccountName))
            .Select(account => account.AccountName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matchedStocks = context.WatchlistItems
            .Where(item => GetStockAliases(item.StockName).Any(alias => ContainsPhrase(question, alias)))
            .Select(item => item.StockName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var wantsAccounts = matchedAccounts.Count > 0 || ContainsAny(question, "account", "balance", "exposure", "cash");
        var wantsTrades = matchedAccounts.Count > 0 || matchedStocks.Count > 0 ||
            ContainsAny(question, "trade", "position", "profit", "loss", "winner", "loser", "open", "closed", "perform");
        var wantsWatchlist = matchedStocks.Count > 0 ||
            ContainsAny(question, "watchlist", "watch", "note", "notes", "tracking", "added");
        var wantsSignals = matchedStocks.Count > 0 ||
            ContainsAny(question, "signal", "signals", "long", "short", "direction");

        if (!wantsAccounts && !wantsTrades && !wantsWatchlist && !wantsSignals)
        {
            wantsAccounts = true;
            wantsTrades = true;
            wantsWatchlist = true;
            wantsSignals = true;
        }

        var sections = new List<string>();

        if (wantsAccounts)
        {
            var section = BuildAccountsSection(context.Accounts, context.Trades, matchedAccounts);
            if (!string.IsNullOrWhiteSpace(section))
            {
                sections.Add(section);
            }
        }

        if (wantsTrades)
        {
            var section = BuildTradesSection(context.Trades, matchedAccounts, matchedStocks);
            if (!string.IsNullOrWhiteSpace(section))
            {
                sections.Add(section);
            }
        }

        if (wantsWatchlist)
        {
            var section = BuildWatchlistSection(context.WatchlistItems, matchedStocks);
            if (!string.IsNullOrWhiteSpace(section))
            {
                sections.Add(section);
            }
        }

        if (wantsSignals)
        {
            var section = BuildSignalsSection(context.TradeSignals, matchedStocks);
            if (!string.IsNullOrWhiteSpace(section))
            {
                sections.Add(section);
            }
        }

        if (sections.Count == 0)
        {
            sections.Add(BuildPortfolioSummary(context));
        }

        return new ChatAnswerDto(
            question,
            string.Join(Environment.NewLine + Environment.NewLine, sections),
            usedTools,
            matchedAccounts,
            matchedStocks);
    }

    private static string BuildAccountsSection(IReadOnlyList<AccountDto> accounts, IReadOnlyList<TradeDto> trades, List<string> matchedAccounts)
    {
        if (accounts.Count == 0)
        {
            return "## Accounts" + Environment.NewLine + "- No accounts are available.";
        }

        var builder = new StringBuilder("## Accounts");

        if (matchedAccounts.Count > 0)
        {
            foreach (var accountName in matchedAccounts)
            {
                var account = accounts.First(item => item.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                var accountTrades = trades.Where(trade => trade.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase)).ToList();
                var openProfitLoss = accountTrades.Where(trade => trade.CloseDate is null).Sum(trade => trade.ProfitLoss);

                builder.AppendLine()
                    .Append('-')
                    .Append(' ')
                    .Append(account.AccountName)
                    .Append(": balance ")
                    .Append(FormatCurrency(account.AccountBalance))
                    .Append(", open trades ")
                    .Append(account.OpenTradeCount)
                    .Append(", open exposure ")
                    .Append(FormatCurrency(account.OpenExposure))
                    .Append(", open P/L ")
                    .Append(FormatCurrency(openProfitLoss));
            }

            return builder.ToString();
        }

        var totalBalance = accounts.Sum(account => account.AccountBalance);
        var totalExposure = accounts.Sum(account => account.OpenExposure);
        var highestBalance = accounts.MaxBy(account => account.AccountBalance);

        builder.AppendLine()
            .Append("- Total account balance: ")
            .Append(FormatCurrency(totalBalance));
        builder.AppendLine()
            .Append("- Total open exposure: ")
            .Append(FormatCurrency(totalExposure));

        if (highestBalance is not null)
        {
            builder.AppendLine()
                .Append("- Highest balance account: ")
                .Append(highestBalance.AccountName)
                .Append(" at ")
                .Append(FormatCurrency(highestBalance.AccountBalance));
        }

        return builder.ToString();
    }

    private static string BuildTradesSection(IReadOnlyList<TradeDto> trades, List<string> matchedAccounts, List<string> matchedStocks)
    {
        IEnumerable<TradeDto> filteredTrades = trades;

        if (matchedAccounts.Count > 0)
        {
            filteredTrades = filteredTrades.Where(trade => matchedAccounts.Contains(trade.AccountName, StringComparer.OrdinalIgnoreCase));
        }

        if (matchedStocks.Count > 0)
        {
            filteredTrades = filteredTrades.Where(trade => matchedStocks.Contains(trade.StockName, StringComparer.OrdinalIgnoreCase));
        }

        var materializedTrades = filteredTrades.ToList();
        if (materializedTrades.Count == 0)
        {
            return "## Trades" + Environment.NewLine + "- No matching trades were found.";
        }

        var openTrades = materializedTrades.Where(trade => trade.CloseDate is null).ToList();
        var closedTrades = materializedTrades.Where(trade => trade.CloseDate is not null).ToList();
        var bestTrade = materializedTrades.MaxBy(trade => trade.ProfitLoss);
        var worstTrade = materializedTrades.MinBy(trade => trade.ProfitLoss);

        var builder = new StringBuilder("## Trades");
        builder.AppendLine()
            .Append("- Matching trades: ")
            .Append(materializedTrades.Count)
            .Append(" total (")
            .Append(openTrades.Count)
            .Append(" open, ")
            .Append(closedTrades.Count)
            .Append(" closed).");
        builder.AppendLine()
            .Append("- Open trade P/L: ")
            .Append(FormatCurrency(openTrades.Sum(trade => trade.ProfitLoss)));
        builder.AppendLine()
            .Append("- Closed trade P/L: ")
            .Append(FormatCurrency(closedTrades.Sum(trade => trade.ProfitLoss)));

        if (bestTrade is not null)
        {
            builder.AppendLine()
                .Append("- Best trade: ")
                .Append(bestTrade.StockName)
                .Append(" in ")
                .Append(bestTrade.AccountName)
                .Append(" at ")
                .Append(FormatCurrency(bestTrade.ProfitLoss));
        }

        if (worstTrade is not null)
        {
            builder.AppendLine()
                .Append("- Worst trade: ")
                .Append(worstTrade.StockName)
                .Append(" in ")
                .Append(worstTrade.AccountName)
                .Append(" at ")
                .Append(FormatCurrency(worstTrade.ProfitLoss));
        }

        return builder.ToString();
    }

    private static string BuildWatchlistSection(IReadOnlyList<WatchlistItemDto> watchlistItems, List<string> matchedStocks)
    {
        var materializedItems = matchedStocks.Count > 0
            ? watchlistItems.Where(item => matchedStocks.Contains(item.StockName, StringComparer.OrdinalIgnoreCase)).ToList()
            : watchlistItems.ToList();

        if (materializedItems.Count == 0)
        {
            return "## Watchlist" + Environment.NewLine + "- No matching watchlist items were found.";
        }

        var builder = new StringBuilder("## Watchlist");

        if (matchedStocks.Count > 0)
        {
            foreach (var item in materializedItems)
            {
                builder.AppendLine()
                    .Append("- ")
                    .Append(item.StockName)
                    .Append(": added ")
                    .Append(item.DateAdded)
                    .Append(", ")
                    .Append(item.SignalCount)
                    .Append(" signals, ")
                    .Append(item.OpenTradeCount)
                    .Append(" open trades. Notes: ")
                    .Append(item.Notes);
            }

            return builder.ToString();
        }

        var latestAdded = materializedItems.MaxBy(item => item.DateAdded);
        builder.AppendLine()
            .Append("- Total watchlist names: ")
            .Append(materializedItems.Count);

        if (latestAdded is not null)
        {
            builder.AppendLine()
                .Append("- Most recently added: ")
                .Append(latestAdded.StockName)
                .Append(" on ")
                .Append(latestAdded.DateAdded);
        }

        return builder.ToString();
    }

    private static string BuildSignalsSection(IReadOnlyList<TradeSignalDto> tradeSignals, List<string> matchedStocks)
    {
        var materializedSignals = matchedStocks.Count > 0
            ? tradeSignals.Where(signal => matchedStocks.Contains(signal.StockName, StringComparer.OrdinalIgnoreCase)).ToList()
            : tradeSignals.ToList();

        if (materializedSignals.Count == 0)
        {
            return "## Trade signals" + Environment.NewLine + "- No matching trade signals were found.";
        }

        var builder = new StringBuilder("## Trade signals");

        if (matchedStocks.Count > 0)
        {
            foreach (var signal in materializedSignals.OrderByDescending(signal => signal.SignalDate))
            {
                builder.AppendLine()
                    .Append("- ")
                    .Append(signal.StockName)
                    .Append(": ")
                    .Append(signal.Direction)
                    .Append(" on ")
                    .Append(signal.SignalDate);
            }

            return builder.ToString();
        }

        var latestSignal = materializedSignals.MaxBy(signal => signal.SignalDate);
        builder.AppendLine()
            .Append("- Total signals: ")
            .Append(materializedSignals.Count)
            .Append(" (")
            .Append(materializedSignals.Count(signal => signal.Direction.ToString().Equals("Long", StringComparison.Ordinal)))
            .Append(" long, ")
            .Append(materializedSignals.Count(signal => signal.Direction.ToString().Equals("Short", StringComparison.Ordinal)))
            .Append(" short).");

        if (latestSignal is not null)
        {
            builder.AppendLine()
                .Append("- Latest signal: ")
                .Append(latestSignal.StockName)
                .Append(' ')
                .Append(latestSignal.Direction)
                .Append(" on ")
                .Append(latestSignal.SignalDate);
        }

        return builder.ToString();
    }

    private static string BuildPortfolioSummary(TradeChatContext context)
    {
        var openTrades = context.Trades.Where(trade => trade.CloseDate is null).ToList();
        var bestOpenTrade = openTrades.MaxBy(trade => trade.ProfitLoss);

        var builder = new StringBuilder("## Portfolio summary");
        builder.AppendLine()
            .Append("- Accounts: ")
            .Append(context.Accounts.Count)
            .Append(", total balance ")
            .Append(FormatCurrency(context.Accounts.Sum(account => account.AccountBalance)));
        builder.AppendLine()
            .Append("- Watchlist names: ")
            .Append(context.WatchlistItems.Count)
            .Append(", signals: ")
            .Append(context.TradeSignals.Count);
        builder.AppendLine()
            .Append("- Trades: ")
            .Append(openTrades.Count)
            .Append(" open and ")
            .Append(context.Trades.Count - openTrades.Count)
            .Append(" closed.");

        if (bestOpenTrade is not null)
        {
            builder.AppendLine()
                .Append("- Best open trade right now: ")
                .Append(bestOpenTrade.StockName)
                .Append(" at ")
                .Append(FormatCurrency(bestOpenTrade.ProfitLoss));
        }

        return builder.ToString();
    }

    private static bool ContainsAny(string input, params string[] values) =>
        values.Any(value => ContainsPhrase(input, value));

    private static bool ContainsPhrase(string input, string value) =>
        input.Contains(value, StringComparison.OrdinalIgnoreCase);

    private static List<string> GetStockAliases(string stockName)
    {
        var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            stockName,
        };

        var parenthesesIndex = stockName.IndexOf('(');
        if (parenthesesIndex > 0)
        {
            aliases.Add(stockName[..parenthesesIndex].Trim());
            var closingIndex = stockName.IndexOf(')', parenthesesIndex + 1);
            if (closingIndex > parenthesesIndex)
            {
                aliases.Add(stockName[(parenthesesIndex + 1)..closingIndex].Trim());
            }
        }

        return aliases.ToList();
    }

    private static string FormatCurrency(decimal value) =>
        value.ToString("C", CultureInfo.InvariantCulture);
}
