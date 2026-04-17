using Trade.Application.Chat;

namespace Trade.Mcp.Api.Features.Chat;

internal interface ITradeQuestionService
{
    Task<ChatAnswerDto> AskAsync(AskTradeQuestionRequest request, CancellationToken cancellationToken);
}
