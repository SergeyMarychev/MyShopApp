using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.ParentCategories.Dto;
using MyShopApp.Application.ParentCategories;

namespace MyShopApp.WebApi.Controllers
{
    public sealed class ParentCategoryController : BaseApiController
    {
        private readonly IParentCategoryAppService _parentCategoryAppService;
        private readonly ILogger<ParentCategoryController> _logger;

        public ParentCategoryController( IParentCategoryAppService parentCategoryAppService, ILogger<ParentCategoryController> logger)
        {
            _parentCategoryAppService = parentCategoryAppService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех родительских категорий
        /// </summary>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение списка всех родительских категорий.");

            var result = await _parentCategoryAppService.GetAllAsync(ct);

            _logger.LogInformation("Отправлен ответ: найдено {Count} родительских категорий.", result.Count());

            return Ok(result);
        }

        /// <summary>
        /// Получение родительской категории по ID
        /// </summary>
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: получение родительской категории по ID = {Id}.", id);

            var result = await _parentCategoryAppService.GetAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: родительская категория ID = {Id} успешно получена.", id);

            return Ok(result);
        }

        /// <summary>
        /// Создание родительской категории
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateParentCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: создание новой родительской категории. Название: {Name}.", input.Name);

            var result = await _parentCategoryAppService.CreateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: родительская категория создана успешно: ID = {Id}.", result.Id);

            return Ok(result);
        }

        /// <summary>
        /// Обновление родительской категории
        /// </summary>
        [HttpPut("[action]")]
        public async Task<IActionResult> Update(UpdateParentCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: обновление родительской категории ID = {Id}.", input.Id);

            var result = await _parentCategoryAppService.UpdateAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: родительская категория ID = {Id} успешно обновлена.", input.Id);

            return Ok(result);
        }

        /// <summary>
        /// Удаление родительской категории (только если нет вложенных категорий)
        /// </summary>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление родительской категории ID = {Id}.", id);

            await _parentCategoryAppService.DeleteAsync(id, ct);

            _logger.LogInformation("Отправлен ответ: родительская категория ID = {Id} успешно удалена.", id);

            return Ok();
        }

        /// <summary>
        /// Добавление категории в родительскую категорию
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCategory(AddCategoryToParentDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: добавление категории ID = {CategoryId} в родительскую категорию ID = {ParentId}.", input.CategoryId, input.ParentCategoryId);

            await _parentCategoryAppService.AddCategoryAsync(input, ct);

            _logger.LogInformation("Отправлен ответ: категория успешно добавлена.");

            return Ok();
        }

        /// <summary>
        /// Удаление категории из родительской категории
        /// </summary>
        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveCategory(long parentCategoryId, long categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получен запрос: удаление категории ID = {CategoryId} из родительской категории ID = {ParentId}.", categoryId, parentCategoryId);

            await _parentCategoryAppService.RemoveCategoryAsync(parentCategoryId, categoryId, ct);

            _logger.LogInformation("Отправлен ответ: категория успешно удалена из родительской категории.");

            return Ok();
        }
    }
}