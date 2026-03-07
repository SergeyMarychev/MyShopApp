using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.ParentCategories
{
    public interface IParentCategoryRepository : IRepository
    {
        /// <summary>
        /// Получает список всех родительских категорий
        /// </summary>
        Task<IEnumerable<ParentCategory>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получает родительскую категорию по идентификатору
        /// </summary>
        Task<ParentCategory?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получает родительскую категорию по названию
        /// </summary>
        Task<ParentCategory?> GetByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Добавляет новую родительскую категорию
        /// </summary>
        Task AddAsync(ParentCategory parentCategory, CancellationToken ct = default);

        /// <summary>
        /// Обновляет существующую родительскую категорию
        /// </summary>
        void Update(ParentCategory parentCategory);

        /// <summary>
        /// Удаляет родительскую категорию
        /// </summary>
        void Delete(ParentCategory parentCategory);
    }
}