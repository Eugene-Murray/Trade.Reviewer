using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Trade.Application.Accounts;
using Trade.Application.Chat;
using Trade.Application.Common.Exceptions;
using Trade.Application.TradeSignals;
using Trade.Application.Trades;
using Trade.Application.Watchlist;
using Trade.Mcp.Api.Mcp;

namespace Trade.Mcp.Api.Features.Chat;

internal sealed class TradeQuestionService(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILoggerFactory loggerFactory,
    ITradeQuestionAnswerBuilder answerBuilder) : ITradeQuestionService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<ChatAnswerDto> AskAsync(AskTradeQuestionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ValidationException("A question is required.");
        }

        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("An active HTTP context is required for MCP chat requests.");

        var endpoint = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/mcp");
        var httpClient = httpClientFactory.CreateClient("trade-mcp-chat");
        await using var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = endpoint,
                Name = "trade-mcp-chat",
                TransportMode = HttpTransportMode.StreamableHttp,
            },
            httpClient,
            loggerFactory);
        await using var client = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);

        var usedTools = new[]
        {
            McpToolNames.GetAccounts,
            McpToolNames.GetTrades,
            McpToolNames.GetWatchlist,
            McpToolNames.GetTradeSignals,
        };

        var context = new TradeChatContext(
            await CallJsonToolAsync<IReadOnlyList<AccountDto>>(client, McpToolNames.GetAccounts, cancellationToken),
            await CallJsonToolAsync<IReadOnlyList<TradeDto>>(client, McpToolNames.GetTrades, cancellationToken),
            await CallJsonToolAsync<IReadOnlyList<WatchlistItemDto>>(client, McpToolNames.GetWatchlist, cancellationToken),
            await CallJsonToolAsync<IReadOnlyList<TradeSignalDto>>(client, McpToolNames.GetTradeSignals, cancellationToken));

        return answerBuilder.BuildAnswer(request.Question.Trim(), context, usedTools);
    }

    private static async Task<T> CallJsonToolAsync<T>(McpClient client, string toolName, CancellationToken cancellationToken)
    {
        var result = await client.CallToolAsync(toolName, cancellationToken: cancellationToken);
        if (result.IsError is true)
        {
            var error = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text
                ?? $"The MCP tool '{toolName}' returned an error.";
            throw new InvalidOperationException(error);
        }

        var json = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text
            ?? throw new InvalidOperationException($"The MCP tool '{toolName}' returned no text content.");

        return JsonSerializer.Deserialize<T>(json, SerializerOptions)
            ?? throw new InvalidOperationException($"The MCP tool '{toolName}' returned invalid JSON.");
    }
}
