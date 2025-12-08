using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            priceBuilder.Property(p => p.Currency)
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.StockQuantity)
            .IsRequired();
    }
}
