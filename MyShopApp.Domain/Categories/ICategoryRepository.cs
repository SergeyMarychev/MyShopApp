using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Categories
{
    public interface ICategoryRepository : IRepository
    {
        /// <summary>
        /// Получение всех категорий
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Коллекция всех категорий</returns>
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение категории по ID
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Категория с указанным ID или null, если не найдена</returns>
        Task<Category?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получение категории по названию
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Категория с указанным названием или null, если не найдена</returns>
        Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="category">Категория для добавления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task AddAsync(Category category, CancellationToken ct = default);

        /// <summary>
        /// Обновление категории
        /// </summary>
        /// <param name="category">Категория для обновления</param>
        void Update(Category category);

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="category">Категория для удаления</param>
        void Delete(Category category);
    }
}