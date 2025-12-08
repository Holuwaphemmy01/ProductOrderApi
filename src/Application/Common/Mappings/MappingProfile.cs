using AutoMapper;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}
