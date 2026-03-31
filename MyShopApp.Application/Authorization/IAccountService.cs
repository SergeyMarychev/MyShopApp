using MyShopApp.Application.Contracts.Authorization.Dto;

namespace MyShopApp.Application.Authorization
{
    public interface IAccountService
    {
        /// <summary>
        /// Запросить код подтверждения для входа
        /// </summary>
        /// <returns>Результат запроса кода (содержит информацию о кулдауне при необходимости)</returns>
        Task<LoginResultDto> RequestCodeAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Проверить код подтверждения и выполнить вход
        /// </summary>
        /// <returns>JWT токен для авторизации</returns>
        Task<TokenDto> VerifyCodeAsync(VerifySmsCodeDto input, CancellationToken ct = default);
    }
}
