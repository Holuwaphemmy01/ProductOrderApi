using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductOrderAPI.Application.Products.Commands.CreateProduct;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Application.Products.Queries.GetProductById;

namespace ProductOrderAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductCommand command)
    {
        var productDto = await _mediator.Send(command);
        return CreatedAtAction("GetProduct", new { id = productDto.Id }, productDto);
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var productDto = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(productDto);
    }
}
