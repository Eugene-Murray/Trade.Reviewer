using System.ComponentModel.DataAnnotations;
using Trade.Application.Accounts;

namespace Trade.Dashboard.Client.Models;

public sealed class AccountFormModel
{
    [Required]
    [StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Range(0, 1_000_000_000)]
    public decimal AccountBalance { get; set; }

    public UpsertAccountRequest ToRequest() => new(AccountName, AccountBalance);

    public static AccountFormModel FromDto(AccountDto dto) => new()
    {
        AccountName = dto.AccountName,
        AccountBalance = dto.AccountBalance,
    };
}
