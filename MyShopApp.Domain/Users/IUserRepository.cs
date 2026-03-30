using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public interface IUserRepository : IRepository
    {
        /// <summary>
        /// Получает пользователя по Id
        /// </summary>
        /// <returns>Сущность пользователя или null, если пользователь не найден</returns>
        Task<User?> GetByIdAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получает пользователя по номеру телефона
        /// </summary>
        /// <returns>Сущность пользователя или null, если пользователь не найден</returns>
        Task<User?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false, CancellationToken ct = default);

        /// <summary>
        /// Получает пользователя по номеру телефона, включая удаленных
        /// </summary>
        /// <returns>Сущность пользователя или null, если пользователь не найден</returns>
        Task<User?> GetByPhoneNumberIncludeDeletedAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Проверяет существование пользователя с указанным номером телефона
        /// </summary>
        /// <returns>True, если пользователь существует, иначе False</returns>
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию добавления</returns>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Обновляет данные существующего пользователя
        /// </summary>
        void Update(User user);

        /// <summary>
        /// Удаляет пользователя (физическое удаление)
        /// </summary>
        void Delete(User user);
    }
}