namespace Trade.Application.Chat;

public interface ITradeQuestionAnswerBuilder
{
    ChatAnswerDto BuildAnswer(string question, TradeChatContext context, IReadOnlyList<string> usedTools);
}
