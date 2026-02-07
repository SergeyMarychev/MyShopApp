using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.Products.Dto;
using MyShopApp.Application.Products;

namespace MyShopApp.WebApi.Controllers
{
    public sealed class ProductController : BaseApiController
    {
        private readonly IProductAppService _productAppService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductAppService productAppService,
            ILogger<ProductController> logger)
        {
            _productAppService = productAppService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех товаров
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение списка всех товаров.");

            var result = await _productAppService.GetAllAsync();

            _logger.LogInformation("Отправлен ответ: найдено {Count} товаров.", result.Count());

            return Ok(result);
        }

        /// <summary>
        /// Получение товара по ID
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение товара по ID = {Id}.", id);

            var product = await _productAppService.GetAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: товар ID = {Id} успешно получен.", id);

            return Ok(product);
        }

        /// <summary>
        /// Создание товара
        /// </summary>
        /// <param name="input">Данные товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateProductDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: создание нового товара. Название: {Name}.", input.Name);

            var product = await _productAppService.CreateAsync(input);

            _logger.LogInformation("Отправлен ответ: товар создан успешно: ID = {Id}.", product.Id);

            return Ok(product);
        }

        /// <summary>
        /// Обновление товара
        /// </summary>
        /// <param name="input">Данные для обновления</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> Update(UpdateProductDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: обновление товара ID = {Id}.", input.Id);

            var product = await _productAppService.UpdateAsync(input);

            _logger.LogInformation("Отправлен ответ: товар ID = {Id} успешно обновлён.", input.Id);

            return Ok(product);
        }

        /// <summary>
        /// Удаление товара
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление товара ID = {Id}.", id);

            await _productAppService.DeleteAsync(id);

            _logger.LogInformation("Отправлен ответ: товар ID = {Id} успешно удалён.", id);

            return Ok();
        }
    }
}
