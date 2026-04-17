using Trade.Application.Chat;

namespace Trade.Mcp.Api.Features.Chat;

internal static class ChatEndpoints
{
    public static RouteGroupBuilder MapChatEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/chat").WithTags("Chat");

        group.MapPost("/ask", async (AskTradeQuestionRequest request, ITradeQuestionService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.AskAsync(request, cancellationToken)));

        return api;
    }
}
