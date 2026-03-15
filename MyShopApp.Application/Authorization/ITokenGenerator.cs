using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Authorization
{
    public interface ITokenGenerator
    {
        Task<TokenDto> GenerateJwtTokenAsync(User user);
    }
}
