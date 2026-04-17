using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trade.Application.Accounts;
using Trade.Application.Chat;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;

namespace Trade.Dashboard.Client.Services;

public sealed class TradeDashboardApiClient(HttpClient httpClient) : ITradeDashboardApiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<AccountDto>("api/accounts", cancellationToken);

    public Task<AccountDto> CreateAccountAsync(UpsertAccountRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<AccountDto>(HttpMethod.Post, "api/accounts", request, cancellationToken);

    public Task<AccountDto> UpdateAccountAsync(int id, UpsertAccountRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<AccountDto>(HttpMethod.Put, $"api/accounts/{id}", request, cancellationToken);

    public Task DeleteAccountAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/accounts/{id}", cancellationToken);

    public Task<IReadOnlyList<WatchlistItemDto>> GetWatchlistAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<WatchlistItemDto>("api/watchlist", cancellationToken);

    public Task<WatchlistItemDto> CreateWatchlistItemAsync(UpsertWatchlistItemRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<WatchlistItemDto>(HttpMethod.Post, "api/watchlist", request, cancellationToken);

    public Task<WatchlistItemDto> UpdateWatchlistItemAsync(int id, UpsertWatchlistItemRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<WatchlistItemDto>(HttpMethod.Put, $"api/watchlist/{id}", request, cancellationToken);

    public Task DeleteWatchlistItemAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/watchlist/{id}", cancellationToken);

    public Task<IReadOnlyList<TradeSignalDto>> GetTradeSignalsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<TradeSignalDto>("api/trade-signals", cancellationToken);

    public Task<TradeSignalDto> CreateTradeSignalAsync(UpsertTradeSignalRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<TradeSignalDto>(HttpMethod.Post, "api/trade-signals", request, cancellationToken);

    public Task<TradeSignalDto> UpdateTradeSignalAsync(int id, UpsertTradeSignalRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<TradeSignalDto>(HttpMethod.Put, $"api/trade-signals/{id}", request, cancellationToken);

    public Task DeleteTradeSignalAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/trade-signals/{id}", cancellationToken);

    public Task<IReadOnlyList<TradeDto>> GetTradesAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<TradeDto>("api/trades", cancellationToken);

    public Task<TradeDto> CreateTradeAsync(UpsertTradeRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<TradeDto>(HttpMethod.Post, "api/trades", request, cancellationToken);

    public Task<TradeDto> UpdateTradeAsync(int id, UpsertTradeRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<TradeDto>(HttpMethod.Put, $"api/trades/{id}", request, cancellationToken);

    public Task DeleteTradeAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/trades/{id}", cancellationToken);

    public Task<ChatAnswerDto> AskTradeQuestionAsync(AskTradeQuestionRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<ChatAnswerDto>(HttpMethod.Post, "api/chat/ask", request, cancellationToken);

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(relativeUrl, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<T>>(SerializerOptions, cancellationToken)
            ?? [];
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string relativeUrl, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, relativeUrl)
        {
            Content = JsonContent.Create(payload),
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<T>(SerializerOptions, cancellationToken)
            ?? throw new ApiException("The API returned an empty response.");
    }

    private async Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        using var response = await httpClient.DeleteAsync(relativeUrl, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var detail = $"The API returned {(int)response.StatusCode}.";
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        if (stream.Length > 0)
        {
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (document.RootElement.TryGetProperty("detail", out var detailProperty) &&
                !string.IsNullOrWhiteSpace(detailProperty.GetString()))
            {
                detail = detailProperty.GetString()!;
            }
            else if (document.RootElement.TryGetProperty("title", out var titleProperty) &&
                     !string.IsNullOrWhiteSpace(titleProperty.GetString()))
            {
                detail = titleProperty.GetString()!;
            }
        }

        throw new ApiException(detail);
    }
}
