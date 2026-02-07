using MyShopApp.Application.Contracts.Products.Dto;

namespace MyShopApp.Application.Products
{
    public interface IProductAppService
    {
        /// <summary>
        /// Получение всех товаров
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список всех товаров с информацией о категориях</returns>
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение товара по ID
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Товар с указанным ID с информацией о категории</returns>
        Task<ProductDto?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Создание товара
        /// </summary>
        /// <param name="input">Данные для создания товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданный товар с присвоенным ID</returns>
        Task<ProductDto> CreateAsync(CreateProductDto input, CancellationToken ct = default);

        /// <summary>
        /// Обновление товара
        /// </summary>
        /// <param name="input">Данные для обновления товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Обновленный товар</returns>
        Task<ProductDto> UpdateAsync(UpdateProductDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаление товара
        /// </summary>
        /// <param name="id">Идентификатор товара для удаления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task DeleteAsync(long id, CancellationToken ct = default);
    }
}