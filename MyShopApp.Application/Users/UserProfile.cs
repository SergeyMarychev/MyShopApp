using AutoMapper;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
        }
    }
}