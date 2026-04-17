using System.Net;
using System.Net.Http.Json;
using Trade.Application.Accounts;
using Trade.Application.Watchlist;

namespace Trade.Mcp.Api.Tests;

public sealed class TradeApiTests
{
    [Fact]
    public async Task GetAccountsReturnsSeededAccounts()
    {
        using var factory = new TradeApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var accounts = await client.GetFromJsonAsync<IReadOnlyList<AccountDto>>("/api/accounts");

        Assert.NotNull(accounts);
        Assert.True(accounts.Count >= 3);
        Assert.Contains(accounts, account => account.AccountName == "Trading Account");
    }

    [Fact]
    public async Task GetWatchlistReturnsSeededNasdaqMegaCaps()
    {
        using var factory = new TradeApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var watchlist = await client.GetFromJsonAsync<IReadOnlyList<WatchlistItemDto>>("/api/watchlist");

        Assert.NotNull(watchlist);
        Assert.Equal(50, watchlist.Count);
        Assert.Contains(watchlist, item => item.StockName.Contains("Apple", StringComparison.Ordinal));
    }

    [Fact]
    public async Task PostAccountCreatesAccount()
    {
        using var factory = new TradeApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var accountName = $"Growth Account {Guid.NewGuid():N}";

        using var response = await client.PostAsJsonAsync("/api/accounts", new UpsertAccountRequest(accountName, 12500m));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<AccountDto>();

        Assert.NotNull(created);
        Assert.Equal(accountName, created.AccountName);
        Assert.Equal(12500m, created.AccountBalance);
    }
}
