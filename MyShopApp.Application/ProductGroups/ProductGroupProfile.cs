using AutoMapper;
using MyShopApp.Application.Contracts.ProductGroups.Dto;
using MyShopApp.Domain.ProductGroups;

namespace MyShopApp.Application.ProductGroups
{
    public class ProductGroupProfile : Profile
    {
        public ProductGroupProfile()
        {
            CreateMap<ProductGroup, ProductGroupDto>().ReverseMap();
            CreateMap<CreateProductGroupDto, ProductGroup>().ReverseMap();
            CreateMap<UpdateProductGroupDto, ProductGroup>().ReverseMap();
        }
    }
}
