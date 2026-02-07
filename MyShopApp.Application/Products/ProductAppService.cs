using AutoMapper;
using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.Products.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Products;

namespace MyShopApp.Application.Products
{
    internal sealed class ProductAppService : IProductAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductAppService> _logger;

        public ProductAppService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper, ILogger<ProductAppService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение списка товаров.");

            var products = await _productRepository.GetAllAsync(ct);

            _logger.LogInformation("Получено {Count} товаров из базы данных.", products.Count());

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение товара ID = {Id}.", id);

            var product = await _productRepository.GetAsync(id, ct);
            if (product == null)
            {
                _logger.LogError("Товар ID = {Id} не найден.", id);
                UserFriendlyException.PRODUCT_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _logger.LogInformation("Товар ID = {Id} успешно найден.", id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато создание товара: Название = {Name}, Категория = {CategoryId}.", input.Name, input.CategoryId);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка создания товара: название отсутствует.");
                UserFriendlyException.PRODUCT_NAME_CAN_NOT_BE_EMPTY();
            }

            var category = await _categoryRepository.GetAsync(input.CategoryId, ct);
            if (category == null)
            {
                _logger.LogError("Ошибка создания товара: категория ID = {Id} не найдена.", input.CategoryId);
                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.CategoryId);
            }

            var product = _mapper.Map<Product>(input);
            product.CreatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product, ct);
            await _productRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Товар успешно создан: ID = {Id}.", product.Id);

            var createdProduct = await _productRepository.GetAsync(product.Id, ct);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато обновление товара ID = {Id}.", input.Id);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка обновления товара ID = {Id}: название отсутствует.", input.Id);
                UserFriendlyException.PRODUCT_NAME_CAN_NOT_BE_EMPTY();
            }

            var category = await _categoryRepository.GetAsync(input.CategoryId, ct);
            if (category == null)
            {
                _logger.LogError(
                    "Ошибка обновления товара ID = {Id}: категория ID = {CategoryId} не найдена.",
                    input.Id, input.CategoryId
                );

                UserFriendlyException.CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.CategoryId);
            }

            var product = await _productRepository.GetAsync(input.Id, ct);
            if (product == null)
            {
                _logger.LogError("Ошибка обновления: товар ID = {Id} не найден.", input.Id);
                UserFriendlyException.PRODUCT_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            _mapper.Map(input, product);
            _productRepository.Update(product);

            _logger.LogInformation("Товар ID = {Id} обновлён успешно.", input.Id);

            var updatedProduct = await _productRepository.GetAsync(product.Id, ct);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление товара ID = {Id}.", id);

            var product = await _productRepository.GetAsync(id, ct);
            if (product == null)
            {
                _logger.LogError("Ошибка удаления: товар ID = {Id} не найден.", id);
                UserFriendlyException.PRODUCT_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _productRepository.Delete(product);

            _logger.LogInformation("Товар ID = {Id} успешно помечен на удаление.", id);
        }
    }
}
