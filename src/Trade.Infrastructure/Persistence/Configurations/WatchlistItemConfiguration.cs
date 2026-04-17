using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trade.Domain.Entities;

namespace Trade.Infrastructure.Persistence.Configurations;

internal sealed class WatchlistItemConfiguration : IEntityTypeConfiguration<WatchlistItem>
{
    public void Configure(EntityTypeBuilder<WatchlistItem> builder)
    {
        builder.ToTable("watchlist");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnName("id");

        builder.Property(item => item.StockName)
            .HasColumnName("stock_name")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(item => item.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(item => item.DateAdded)
            .HasColumnName("date_added");

        builder.HasIndex(item => item.StockName)
            .IsUnique();
    }
}
