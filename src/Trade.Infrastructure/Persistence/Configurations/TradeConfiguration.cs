using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trade.Domain.Entities;
using TradeEntity = Trade.Domain.Entities.Trade;

namespace Trade.Infrastructure.Persistence.Configurations;

internal sealed class TradeConfiguration : IEntityTypeConfiguration<TradeEntity>
{
    public void Configure(EntityTypeBuilder<TradeEntity> builder)
    {
        builder.ToTable("trades");

        builder.HasKey(trade => trade.Id);

        builder.Property(trade => trade.Id)
            .HasColumnName("id");

        builder.Property(trade => trade.StockName)
            .HasColumnName("stock_name")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(trade => trade.EntryPrice)
            .HasColumnName("entry_price")
            .HasPrecision(18, 2);

        builder.Property(trade => trade.CurrentPrice)
            .HasColumnName("current_price")
            .HasPrecision(18, 2);

        builder.Property(trade => trade.ClosePrice)
            .HasColumnName("close_price")
            .HasPrecision(18, 2);

        builder.Property(trade => trade.PositionSize)
            .HasColumnName("position_size")
            .HasPrecision(18, 2);

        builder.Property(trade => trade.OpenDate)
            .HasColumnName("open_date");

        builder.Property(trade => trade.CloseDate)
            .HasColumnName("close_date");

        builder.Property(trade => trade.AccountId)
            .HasColumnName("account_id");

        builder.HasOne(trade => trade.Account)
            .WithMany(account => account.Trades)
            .HasForeignKey(trade => trade.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(trade => trade.WatchlistItem)
            .WithMany(item => item.Trades)
            .HasForeignKey(trade => trade.StockName)
            .HasPrincipalKey(item => item.StockName)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
