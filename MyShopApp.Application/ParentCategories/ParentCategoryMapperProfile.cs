using AutoMapper;
using MyShopApp.Application.Contracts.ParentCategories.Dto;
using MyShopApp.Domain.ParentCategories;

namespace MyShopApp.Application.ParentCategories
{
    public class ParentCategoryMapperProfile : Profile
    {
        public ParentCategoryMapperProfile()
        {
            CreateMap<ParentCategory, ParentCategoryDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

            CreateMap<CreateParentCategoryDto, ParentCategory>();
            CreateMap<UpdateParentCategoryDto, ParentCategory>();
        }
    }
}
