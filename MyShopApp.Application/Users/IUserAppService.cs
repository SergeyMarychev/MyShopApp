using MyShopApp.Application.Contracts.Email;
using MyShopApp.Application.Contracts.Users;

namespace MyShopApp.Application.Users
{
    public interface IUserAppService
    {
        /// <summary>
        /// Получить профиль пользователя по идентификатору
        /// </summary>
        /// <returns>Данные профиля пользователя</returns>
        Task<UserDto> GetAsync(long userId, CancellationToken ct = default);

        /// <summary>
        /// Обновить профиль пользователя (имя и настройки уведомлений)
        /// </summary>
        /// <remarks>Если указан новый email, на него будет отправлен код подтверждения. Сам email обновится только после подтверждения кода</remarks>
        Task UpdateAsync(UpdateUserDto input, CancellationToken ct = default);

        /// <summary>
        /// Удалить аккаунт пользователя (soft-delete)
        /// </summary>
        Task DeleteAsync(long userId, CancellationToken ct = default);

        /// <summary>
        /// Подтвердить email пользователя
        /// </summary>
        /// <returns>Результат подтверждения email</returns>
        Task<ConfirmEmailResultDto> ConfirmEmailAsync(long userId, ConfirmEmailDto input, CancellationToken ct = default);
    }
}
