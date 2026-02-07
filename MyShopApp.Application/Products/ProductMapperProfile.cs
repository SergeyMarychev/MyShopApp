using AutoMapper;
using MyShopApp.Application.Contracts.Products.Dto;
using MyShopApp.Domain.Products;

namespace MyShopApp.Application.Products
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
