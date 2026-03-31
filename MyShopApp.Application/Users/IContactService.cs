using MyShopApp.Application.Contracts.Users;

namespace MyShopApp.Application.Users
{
    public interface IContactService
    {
        /// <summary>
        /// Обработать обращение пользователя
        /// </summary>
        /// <returns>Результат обработки обращения</returns>
        Task<ContactResponseDto> ContactAsync(long userId, ContactRequestDto input, CancellationToken ct = default);
    }
}
