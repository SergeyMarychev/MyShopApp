using MyShopApp.Application.Contracts.Categories.Dto;

namespace MyShopApp.Application.Categories
{
    public interface ICategoryAppService
    {
        /// <summary>
        /// Получение всех категорий
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список всех категорий</returns>
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение категории по ID
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Категория с указанным ID или null, если не найдена</returns>
        Task<CategoryDto?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="input">Данные для создания категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданная категория</returns>
        Task<CategoryDto> CreateAsync(CreateCategoryDto input, CancellationToken ct = default);

        /// <summary>
        /// Обновление категории
        /// </summary>
        /// <param name="input">Данные для обновления категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Обновленная категория</returns>
        Task<CategoryDto> UpdateAsync(UpdateCategoryDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="id">Идентификатор категории для удаления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task DeleteAsync(long id, CancellationToken ct = default);
    }
}