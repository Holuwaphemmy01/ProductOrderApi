using ProductOrderAPI.Domain.ValueObjects;

namespace ProductOrderAPI.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Money Price { get; set; }
    public int StockQuantity { get; set; }

    public Product(string name, string description, Money price, int stockQuantity)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }
}
