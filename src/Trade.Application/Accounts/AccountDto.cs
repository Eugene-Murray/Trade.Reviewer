namespace Trade.Application.Accounts;

public sealed record AccountDto(
    int Id,
    string AccountName,
    decimal AccountBalance,
    int OpenTradeCount,
    decimal OpenExposure);
