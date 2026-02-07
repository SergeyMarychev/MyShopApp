using AutoMapper;
using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.Categories.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Categories;

namespace MyShopApp.Application.Categories
{
    internal sealed class CategoryAppService : ICategoryAppService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryAppService> _logger;

        public CategoryAppService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryAppService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение списка категорий.");

            var categories = await _categoryRepository.GetAllAsync(ct);

            _logger.LogInformation("Получено {Count} категорий из базы данных.", categories.Count());

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение категории ID = {Id}.", id);

            var category = await _categoryRepository.GetAsync(id, ct);
            if (category == null)
            {
                _logger.LogError("Категория ID = {Id} не найдена.", id);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _logger.LogInformation("Категория ID = {Id} успешно найдена.", id);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато создание категории: Название = {Name}.", input.Name);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка создания категории: название отсутствует.");
                UserFriendlyException.CATEGORY_NAME_CAN_NOT_BE_EMPTY();
            }

            var existingCategory = await _categoryRepository.GetByNameAsync(input.Name, ct);
            if (existingCategory != null)
            {
                _logger.LogError("Ошибка создания категории: категория с названием '{Name}' уже существует.", input.Name);
                UserFriendlyException.CATEGORY_WITH_NAME_ALREADY_EXISTS(input.Name);
            }

            var category = _mapper.Map<Category>(input);
            category.CreatedAt = DateTime.UtcNow;

            await _categoryRepository.AddAsync(category, ct);
            await _categoryRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Категория успешно создана: ID = {Id}, Название = {Name}.", category.Id, category.Name);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато обновление категории ID = {Id}.", input.Id);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка обновления категории ID = {Id}: название отсутствует.", input.Id);
                UserFriendlyException.CATEGORY_NAME_CAN_NOT_BE_EMPTY();
            }

            var category = await _categoryRepository.GetAsync(input.Id, ct);
            if (category == null)
            {
                _logger.LogError("Ошибка обновления: категория ID = {Id} не найдена.", input.Id);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            var existingCategory = await _categoryRepository.GetByNameAsync(input.Name, ct);
            if (existingCategory != null)
            {
                _logger.LogError(
                    "Ошибка обновления категории ID = {Id}: категория с названием '{Name}' уже существует.",
                    input.Id, input.Name
                );
                UserFriendlyException.CATEGORY_WITH_NAME_ALREADY_EXISTS(input.Name);
            }

            _mapper.Map(input, category);
            _categoryRepository.Update(category);

            _logger.LogInformation("Категория ID = {Id} обновлена успешно.", input.Id);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление категории ID = {Id}.", id);

            var category = await _categoryRepository.GetAsync(id, ct);
            if (category == null)
            {
                _logger.LogError("Ошибка удаления: категория ID = {Id} не найдена.", id);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _categoryRepository.Delete(category);

            _logger.LogInformation("Категория ID = {Id} успешно помечена на удаление.", id);
        }
    }
}
