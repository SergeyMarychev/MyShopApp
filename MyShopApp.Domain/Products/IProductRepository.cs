using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Products
{
    /// <summary>
    /// Репозиторий для работы с товарами
    /// </summary>
    public interface IProductRepository : IRepository
    {
        /// <summary>
        /// Получение всех товаров
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Коллекция всех товаров с включенными категориями</returns>
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение товара по ID
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Товар с указанным ID с включенной категорией или null, если не найден</returns>
        Task<Product?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Получение товара по названию
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Товар с указанным названием с включенной категорией или null, если не найден</returns>
        Task<Product?> GetByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Добавление товара
        /// </summary>
        /// <param name="product">Товар для добавления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task AddAsync(Product product, CancellationToken ct = default);

        /// <summary>
        /// Обновление товара
        /// </summary>
        /// <param name="product">Товар для обновления</param>
        void Update(Product product);

        /// <summary>
        /// Удаление товара
        /// </summary>
        /// <param name="product">Товар для удаления</param>
        void Delete(Product product);
    }
}