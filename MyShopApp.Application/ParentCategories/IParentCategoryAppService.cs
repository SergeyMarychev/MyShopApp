using MyShopApp.Application.Contracts.ParentCategories.Dto;

namespace MyShopApp.Application.ParentCategories
{
    public interface IParentCategoryAppService
    {
        /// <summary>
        /// Получает список всех родительских категорий
        /// </summary>
        Task<IEnumerable<ParentCategoryDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получает родительскую категорию по идентификатору
        /// </summary>
        Task<ParentCategoryDto> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Создает новую родительскую категорию
        /// </summary>
        Task<ParentCategoryDto> CreateAsync(CreateParentCategoryDto input, CancellationToken ct = default);

        /// <summary>
        /// Обновляет существующую родительскую категорию
        /// </summary>
        Task<ParentCategoryDto> UpdateAsync(UpdateParentCategoryDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаляет родительскую категорию
        /// </summary>
        Task DeleteAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Добавляет категорию в родительскую категорию
        /// </summary>
        Task AddCategoryAsync(AddCategoryToParentDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаляет категорию из родительской категории
        /// </summary>
        Task RemoveCategoryAsync(long parentCategoryId, long categoryId, CancellationToken ct = default);
    }
}