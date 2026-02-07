using MyShopApp.Application.Contracts.ProductGroups.Dto;

namespace MyShopApp.Application.ProductGroups
{
    public interface IProductGroupAppService
    {
        /// <summary>
        /// Получение всех групп товаров с информацией о товарах
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список всех групп товаров</returns>
        Task<IEnumerable<ProductGroupDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение группы товаров по ID с информацией о товарах
        /// </summary>
        /// <param name="id">Идентификатор группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Группа товаров с указанным ID</returns>
        Task<ProductGroupDto?> GetAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Создание группы товаров
        /// </summary>
        /// <param name="input">Данные для создания группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданная группа товаров с присвоенным ID</returns>
        Task<ProductGroupDto> CreateAsync(CreateProductGroupDto input, CancellationToken ct = default);

        /// <summary>
        /// Обновление группы товаров
        /// </summary>
        /// <param name="input">Данные для обновления группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Обновленная группа товаров</returns>
        Task<ProductGroupDto> UpdateAsync(UpdateProductGroupDto input, CancellationToken ct = default);

        /// <summary>
        /// Удаление группы товаров
        /// </summary>
        /// <param name="id">Идентификатор группы товаров для удаления</param>
        /// <param name="ct">Токен отмены операции</param>
        Task DeleteAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Добавление товара в группу
        /// </summary>
        /// <param name="productGroupId">Идентификатор группы товаров</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="ct">Токен отмены операции</param>
        Task AddProductToGroupAsync(long productGroupId, long productId, CancellationToken ct = default);

        /// <summary>
        /// Удаление товара из группы
        /// </summary>
        /// <param name="productGroupId">Идентификатор группы товаров</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="ct">Токен отмены операции</param>
        Task RemoveProductFromGroupAsync(long productGroupId, long productId, CancellationToken ct = default);
    }
}
