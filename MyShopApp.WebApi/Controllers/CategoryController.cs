using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Categories;
using MyShopApp.Application.Contracts.Categories.Dto;

namespace MyShopApp.WebApi.Controllers
{
    public sealed class CategoryController : BaseApiController
    {
        private readonly ICategoryAppService _categoryAppService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryAppService categoryAppService, ILogger<CategoryController> logger)
        {
            _categoryAppService = categoryAppService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех категорий
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение списка всех категорий.");

            var result = await _categoryAppService.GetAllAsync(ct);

            _logger.LogInformation("Отправлен ответ: найдено {Count} категорий.", result.Count());

            return Ok(result);
        }

        /// <summary>
        /// Получение категории по ID
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение категории по ID = {Id}.", id);

            var category = await _categoryAppService.GetAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: категория ID = {Id} успешно получена.", id);

            return Ok(category);
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="input">Данные категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: создание новой категории. Название: {Name}.", input.Name);

            var category = await _categoryAppService.CreateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: категория создана успешно: ID = {Id}.", category.Id);

            return Ok(category);
        }

        /// <summary>
        /// Обновление категории
        /// </summary>
        /// <param name="input">Данные для обновления</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> Update(UpdateCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: обновление категории ID = {Id}.", input.Id);

            var category = await _categoryAppService.UpdateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: категория ID = {Id} успешно обновлена.", input.Id);

            return Ok(category);
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление категории ID = {Id}.", id);

            await _categoryAppService.DeleteAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: категория ID = {Id} успешно удалена.", id);

            return Ok();
        }
    }
}
