using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.ProductGroups
{
    public interface IProductGroupRepository : IRepository
    {
        /// <summary>
        /// Получение всех групп товаров с включенными товарами
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Коллекция всех групп товаров</returns>
        Task<IEnumerable<ProductGroup>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение группы товаров по ID с включенными товарами
        /// </summary>
        /// <param name="id">Идентификатор группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Группа товаров с указанным ID или null, если не найдена</returns>
        Task<ProductGroup?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получение группы товаров по названию
        /// </summary>
        /// <param name="name">Название группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Группа товаров с указанным названием или null, если не найдена</returns>
        Task<ProductGroup?> GetByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Добавление группы товаров
        /// </summary>
        /// <param name="productGroup">Группа товаров для добавления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task AddAsync(ProductGroup productGroup, CancellationToken ct = default);

        /// <summary>
        /// Обновление группы товаров
        /// </summary>
        /// <param name="productGroup">Группа товаров для обновления</param>
        void Update(ProductGroup productGroup);

        /// <summary>
        /// Удаление группы товаров
        /// </summary>
        /// <param name="productGroup">Группа товаров для удаления</param>
        void Delete(ProductGroup productGroup);
    }
}
