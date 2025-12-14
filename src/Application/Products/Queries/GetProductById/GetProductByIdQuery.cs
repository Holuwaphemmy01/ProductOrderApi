using MediatR;
using ProductOrderAPI.Application.Products.DTOs;

namespace ProductOrderAPI.Application.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
}
