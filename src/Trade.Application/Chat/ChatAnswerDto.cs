namespace Trade.Application.Chat;

public sealed record ChatAnswerDto(
    string Question,
    string Answer,
    IReadOnlyList<string> UsedTools,
    IReadOnlyList<string> ReferencedAccounts,
    IReadOnlyList<string> ReferencedStocks);
