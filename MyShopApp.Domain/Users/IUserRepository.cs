using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public interface IUserRepository : IRepository
    {
        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <returns>Пользователь или null, если не найден</returns>
        Task<User?> GetByIdAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получить пользователя по номеру телефона
        /// </summary>
        /// <returns>Пользователь или null, если не найден</returns>
        Task<User?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false, CancellationToken ct = default);

        /// <summary>
        /// Получить пользователя по номеру телефона (включая удаленных)
        /// </summary>
        /// <returns>Пользователь или null, если не найден</returns>
        Task<User?> GetByPhoneNumberIncludeDeletedAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Проверить существование пользователя с указанным номером телефона
        /// </summary>
        /// <returns>true - если пользователь существует, иначе false</returns>
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Обновить существующего пользователя
        /// </summary>
        void Update(User user);

        /// <summary>
        /// Удалить пользователя (hard delete)
        /// </summary>
        void Delete(User user);
    }
}