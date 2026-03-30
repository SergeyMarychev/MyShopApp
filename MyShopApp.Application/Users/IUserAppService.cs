using MyShopApp.Application.Contracts.Users;

namespace MyShopApp.Application.Users
{
    public interface IUserAppService
    {
        /// <summary>
        /// Получает информацию о текущем пользователе, включая имя, номер телефона и последний адрес
        /// </summary>
        /// <returns>DTO с информацией о текущем пользователе</returns>
        Task<CurrentUserInfoDto> GetCurrentUserInfoAsync(long userId, CancellationToken ct = default);

        /// <summary>
        /// Получает профиль пользователя по идентификатору
        /// </summary>
        /// <returns>DTO с данными профиля пользователя</returns>
        Task<UserDto> GetAsync(long userId, CancellationToken ct = default);

        /// <summary>
        /// Обновляет данные профиля пользователя
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию обновления</returns>
        Task UpdateAsync(UpdateUserDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаляет аккаунт пользователя (мягкое удаление)
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию удаления</returns>
        Task DeleteAsync(long userId, CancellationToken ct = default);
    }
}