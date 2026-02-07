using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.ProductGroups.Dto;
using MyShopApp.Application.ProductGroups;

namespace MyShopApp.WebApi.Controllers
{
    public sealed class ProductGroupController : BaseApiController
    {
        private readonly IProductGroupAppService _productGroupAppService;
        private readonly ILogger<ProductGroupController> _logger;

        public ProductGroupController(IProductGroupAppService productGroupAppService, ILogger<ProductGroupController> logger)
        {
            _productGroupAppService = productGroupAppService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех групп товаров
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список групп товаров</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение списка всех групп товаров.");

            var result = await _productGroupAppService.GetAllAsync(ct);

            _logger.LogInformation("Отправлен ответ: найдено {Count} групп товаров.", result.Count());

            return Ok(result);
        }

        /// <summary>
        /// Получение группы товаров по ID
        /// </summary>
        /// <param name="id">ID группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Группа товаров</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение группы товаров по ID = {Id}.", id);

            var productGroup = await _productGroupAppService.GetAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: группа товаров ID = {Id} успешно получена.", id);

            return Ok(productGroup);
        }

        /// <summary>
        /// Создание группы товаров
        /// </summary>
        /// <param name="input">Данные группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданная группа товаров</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateProductGroupDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: создание новой группы товаров. Название: {Name}.", input.Name);

            var productGroup = await _productGroupAppService.CreateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: группа товаров создана успешно: ID = {Id}.", productGroup.Id);

            return Ok(productGroup);
        }

        /// <summary>
        /// Обновление группы товаров
        /// </summary>
        /// <param name="input">Данные для обновления</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Обновленная группа товаров</returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> Update(UpdateProductGroupDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: обновление группы товаров ID = {Id}.", input.Id);

            var productGroup = await _productGroupAppService.UpdateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: группа товаров ID = {Id} успешно обновлена.", input.Id);

            return Ok(productGroup);
        }

        /// <summary>
        /// Удаление группы товаров
        /// </summary>
        /// <param name="id">ID группы товаров</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление группы товаров ID = {Id}.", id);

            await _productGroupAppService.DeleteAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: группа товаров ID = {Id} успешно удалена.", id);

            return Ok();
        }

        /// <summary>
        /// Добавление товара в группу
        /// </summary>
        /// <param name="productGroupId">ID группы товаров</param>
        /// <param name="productId">ID товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddProduct(long productGroupId, long productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: добавление товара ID = {ProductId} в группу ID = {GroupId}.", productId, productGroupId);

            await _productGroupAppService.AddProductToGroupAsync(productGroupId, productId, ct);

            _logger.LogInformation("Отправлен ответ: товар ID = {ProductId} успешно добавлен в группу ID = {GroupId}.", productId, productGroupId);

            return Ok();
        }

        /// <summary>
        /// Удаление товара из группы
        /// </summary>
        /// <param name="productGroupId">ID группы товаров</param>
        /// <param name="productId">ID товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveProduct(long productGroupId, long productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление товара ID = {ProductId} из группы ID = {GroupId}.", productId, productGroupId);

            await _productGroupAppService.RemoveProductFromGroupAsync(productGroupId, productId, ct);

            _logger.LogInformation("Отправлен ответ: товар ID = {ProductId} успешно удален из группы ID = {GroupId}.", productId, productGroupId);

            return Ok();
        }
    }
}