using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Authorization
{
    public interface ITokenGenerator
    {
        /// <summary>
        /// Сгенерировать JWT токен для пользователя
        /// </summary>
        /// <returns>JWT токен с данными авторизации</returns>
        Task<TokenDto> GenerateJwtTokenAsync(User user);
    }
}
