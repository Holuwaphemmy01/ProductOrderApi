using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductOrderAPI.Application.Common.Interfaces;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.Exceptions;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(productEntity => productEntity.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        product.Update(request.Name, request.Description, request.Price, request.StockQuantity);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
