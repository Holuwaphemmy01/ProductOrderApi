using MediatR;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.ValueObjects;

namespace ProductOrderAPI.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, Money Price, int StockQuantity) : IRequest<ProductDto>;
