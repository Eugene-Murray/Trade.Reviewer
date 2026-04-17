using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trade.Domain.Entities;

namespace Trade.Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Id)
            .HasColumnName("id");

        builder.Property(account => account.AccountName)
            .HasColumnName("account_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(account => account.AccountBalance)
            .HasColumnName("account_balance")
            .HasPrecision(18, 2);

        builder.HasIndex(account => account.AccountName)
            .IsUnique();
    }
}
