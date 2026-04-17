using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trade.Domain.Entities;
using Trade.Domain.Enums;

namespace Trade.Infrastructure.Persistence.Configurations;

internal sealed class TradeSignalConfiguration : IEntityTypeConfiguration<TradeSignal>
{
    public void Configure(EntityTypeBuilder<TradeSignal> builder)
    {
        builder.ToTable("trade_signals");

        builder.HasKey(signal => signal.Id);

        builder.Property(signal => signal.Id)
            .HasColumnName("id");

        builder.Property(signal => signal.StockName)
            .HasColumnName("stock_name")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(signal => signal.Direction)
            .HasColumnName("direction")
            .HasConversion(
                direction => direction == TradeDirection.Long ? "long" : "short",
                value => value == "short" ? TradeDirection.Short : TradeDirection.Long)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(signal => signal.SignalDate)
            .HasColumnName("signal_date");

        builder.HasOne(signal => signal.WatchlistItem)
            .WithMany(item => item.TradeSignals)
            .HasForeignKey(signal => signal.StockName)
            .HasPrincipalKey(item => item.StockName)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
