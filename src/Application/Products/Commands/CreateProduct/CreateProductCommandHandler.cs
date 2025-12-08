using AutoMapper;
using MediatR;
using ProductOrderAPI.Application.Common.Interfaces;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Description, request.Price, request.StockQuantity);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
