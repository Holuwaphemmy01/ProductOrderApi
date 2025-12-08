using ProductOrderAPI.Domain.ValueObjects;

namespace ProductOrderAPI.Application.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Money? Price { get; set; }
    public int StockQuantity { get; set; }
}
