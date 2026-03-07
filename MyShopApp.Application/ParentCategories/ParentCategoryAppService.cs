using AutoMapper;
using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.ParentCategories.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Categories;
using MyShopApp.Domain.ParentCategories;

namespace MyShopApp.Application.ParentCategories
{
    internal sealed class ParentCategoryAppService : IParentCategoryAppService
    {
        private readonly IParentCategoryRepository _parentCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ParentCategoryAppService> _logger;

        public ParentCategoryAppService(
            IParentCategoryRepository parentCategoryRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<ParentCategoryAppService> logger)
        {
            _parentCategoryRepository = parentCategoryRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ParentCategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение списка родительских категорий.");

            var parentCategories = await _parentCategoryRepository.GetAllAsync(ct);

            _logger.LogInformation("Получено {Count} родительских категорий из базы данных.", parentCategories.Count());

            return _mapper.Map<IEnumerable<ParentCategoryDto>>(parentCategories);
        }

        public async Task<ParentCategoryDto> GetAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение родительской категории ID = {Id}.", id);

            var parentCategory = await _parentCategoryRepository.GetAsync(id, ct);
            if (parentCategory == null)
            {
                _logger.LogError("Родительская категория ID = {Id} не найдена.", id);
                UserFriendlyException.PARENT_CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _logger.LogInformation("Родительская категория ID = {Id} успешно найдена.", id);

            return _mapper.Map<ParentCategoryDto>(parentCategory);
        }

        public async Task<ParentCategoryDto> CreateAsync(CreateParentCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато создание родительской категории: Название = {Name}.", input.Name);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка создания родительской категории: название отсутствует.");
                UserFriendlyException.PARENT_CATEGORY_NAME_CAN_NOT_BE_EMPTY();
            }

            var existingParentCategory = await _parentCategoryRepository.GetByNameAsync(input.Name, ct);
            if (existingParentCategory != null)
            {
                _logger.LogError("Ошибка создания родительской категории: категория с названием '{Name}' уже существует.", input.Name);
                UserFriendlyException.PARENT_CATEGORY_WITH_NAME_ALREADY_EXISTS(input.Name);
            }

            var parentCategory = _mapper.Map<ParentCategory>(input);
            parentCategory.CreatedAt = DateTime.UtcNow;

            await _parentCategoryRepository.AddAsync(parentCategory, ct);
            await _parentCategoryRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Родительская категория успешно создана: ID = {Id}, Название = {Name}.", parentCategory.Id, parentCategory.Name);

            return _mapper.Map<ParentCategoryDto>(parentCategory);
        }

        public async Task<ParentCategoryDto> UpdateAsync(UpdateParentCategoryDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато обновление родительской категории ID = {Id}.", input.Id);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка обновления родительской категории ID = {Id}: название отсутствует.", input.Id);
                UserFriendlyException.PARENT_CATEGORY_NAME_CAN_NOT_BE_EMPTY();
            }

            var parentCategory = await _parentCategoryRepository.GetAsync(input.Id, ct);
            if (parentCategory == null)
            {
                _logger.LogError("Ошибка обновления: родительская категория ID = {Id} не найдена.", input.Id);
                UserFriendlyException.PARENT_CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            var existingParentCategory = await _parentCategoryRepository.GetByNameAsync(input.Name, ct);
            if (existingParentCategory != null && existingParentCategory.Id != input.Id)
            {
                _logger.LogError("Ошибка обновления родительской категории ID = {Id}: категория с названием '{Name}' уже существует.", input.Id, input.Name);
                UserFriendlyException.PARENT_CATEGORY_WITH_NAME_ALREADY_EXISTS(input.Name);
            }

            _mapper.Map(input, parentCategory);
            _parentCategoryRepository.Update(parentCategory);

            _logger.LogInformation("Родительская категория ID = {Id} обновлена успешно.", input.Id);

            return _mapper.Map<ParentCategoryDto>(parentCategory);
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление родительской категории ID = {Id}.", id);

            // Получаем родительскую категорию
            var parentCategory = await _parentCategoryRepository.GetAsync(id, ct);
            if (parentCategory == null)
            {
                _logger.LogError("Ошибка удаления: родительская категория ID = {Id} не найдена.", id);
                UserFriendlyException.PARENT_CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            // Получаем все категории и проверяем, есть ли привязанные к этой родительской категории
            var allCategories = await _categoryRepository.GetAllAsync(ct);
            var categoriesInParent = allCategories.Where(c => c.ParentCategoryId == id);

            if (categoriesInParent.Any())
            {
                _logger.LogWarning("Родительская категория ID = {Id} содержит {Count} категорий. Удаление невозможно.", id, categoriesInParent.Count());
                UserFriendlyException.PARENT_CATEGORY_CANNOT_BE_DELETED_HAS_CATEGORIES();
            }

            _parentCategoryRepository.Delete(parentCategory);
            await _parentCategoryRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Родительская категория ID = {Id} успешно удалена.", id);
        }

        public async Task AddCategoryAsync(AddCategoryToParentDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато добавление категории ID = {CategoryId} в родительскую категорию ID = {ParentId}.", input.CategoryId, input.ParentCategoryId);

            var parentCategory = await _parentCategoryRepository.GetAsync(input.ParentCategoryId, ct);
            if (parentCategory == null)
            {
                _logger.LogError("Родительская категория ID = {Id} не найдена.", input.ParentCategoryId);
                UserFriendlyException.PARENT_CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.ParentCategoryId);
            }

            var category = await _categoryRepository.GetAsync(input.CategoryId, ct);
            if (category == null)
            {
                _logger.LogError("Категория ID = {Id} не найдена.", input.CategoryId);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.CategoryId);
            }

            // Проверяем, не привязана ли уже категория к этой родительской категории
            if (category.ParentCategoryId == input.ParentCategoryId)
            {
                _logger.LogWarning("Категория ID = {CategoryId} уже добавлена в родительскую категорию ID = {ParentId}.", input.CategoryId, input.ParentCategoryId);
                UserFriendlyException.CATEGORY_ALREADY_IN_PARENT();
            }

            category.ParentCategoryId = input.ParentCategoryId;
            _categoryRepository.Update(category);
            await _categoryRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Категория ID = {CategoryId} успешно добавлена в родительскую категорию ID = {ParentId}.", input.CategoryId, input.ParentCategoryId);
        }

        public async Task RemoveCategoryAsync(long parentCategoryId, long categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление категории ID = {CategoryId} из родительской категории ID = {ParentId}.", categoryId, parentCategoryId);

            var parentCategory = await _parentCategoryRepository.GetAsync(parentCategoryId, ct);
            if (parentCategory == null)
            {
                _logger.LogError("Родительская категория ID = {Id} не найдена.", parentCategoryId);
                UserFriendlyException.PARENT_CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(parentCategoryId);
            }

            var category = await _categoryRepository.GetAsync(categoryId, ct);
            if (category == null)
            {
                _logger.LogError("Категория ID = {Id} не найдена.", categoryId);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(categoryId);
            }

            if (category.ParentCategoryId != parentCategoryId)
            {
                _logger.LogWarning("Категория ID = {CategoryId} не принадлежит родительской категории ID = {ParentId}.", categoryId, parentCategoryId);
                UserFriendlyException.CATEGORY_NOT_IN_PARENT();
            }

            category.ParentCategoryId = null;
            _categoryRepository.Update(category);
            await _categoryRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Категория ID = {CategoryId} успешно удалена из родительской категории ID = {ParentId}.", categoryId, parentCategoryId);
        }
    }
}