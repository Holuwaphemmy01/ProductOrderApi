using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(1000);

        builder.OwnsOne(product => product.Price, priceBuilder =>
        {
            priceBuilder.Property(price => price.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            priceBuilder.Property(price => price.Currency)
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(product => product.StockQuantity)
            .IsRequired();
    }
}
