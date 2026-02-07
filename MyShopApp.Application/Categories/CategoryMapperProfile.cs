using AutoMapper;
using MyShopApp.Application.Contracts.Categories.Dto;
using MyShopApp.Domain.Categories;

namespace MyShopApp.Application.Categories
{
    public class CategoryMapperProfile : Profile
    {
        public CategoryMapperProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}
