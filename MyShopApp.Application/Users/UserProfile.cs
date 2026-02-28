using AutoMapper;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();
        }
    }
}
