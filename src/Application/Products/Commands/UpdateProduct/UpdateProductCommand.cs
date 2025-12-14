using MediatR;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.ValueObjects;

namespace ProductOrderAPI.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, string Description, Money Price, int StockQuantity) : IRequest<ProductDto>;