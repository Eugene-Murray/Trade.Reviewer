namespace Trade.Domain.Entities;

public sealed class Account
{
    public int Id { get; set; }

    public string AccountName { get; set; } = string.Empty;

    public decimal AccountBalance { get; set; }

    public ICollection<Trade> Trades { get; set; } = [];
}
