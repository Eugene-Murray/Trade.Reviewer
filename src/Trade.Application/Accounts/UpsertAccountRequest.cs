namespace Trade.Application.Accounts;

public sealed record UpsertAccountRequest(
    string AccountName,
    decimal AccountBalance);
