using AutoMapper;
using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.ProductGroups.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.ProductGroups;
using MyShopApp.Domain.Products;

namespace MyShopApp.Application.ProductGroups
{
    internal sealed class ProductGroupAppService : IProductGroupAppService
    {
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductGroupAppService> _logger;

        public ProductGroupAppService(IProductGroupRepository productGroupRepository, IProductRepository productRepository, IMapper mapper, ILogger<ProductGroupAppService> logger)
        {
            _productGroupRepository = productGroupRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductGroupDto>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение списка групп товаров.");

            var productGroups = await _productGroupRepository.GetAllAsync(ct);

            _logger.LogInformation("Получено {Count} групп товаров из базы данных.", productGroups.Count());

            return _mapper.Map<IEnumerable<ProductGroupDto>>(productGroups);
        }

        public async Task<ProductGroupDto?> GetAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато получение группы товаров ID = {Id}.", id);

            var productGroup = await _productGroupRepository.GetAsync(id, ct);
            if (productGroup == null)
            {
                _logger.LogError("Группа товаров ID = {Id} не найдена.", id);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _logger.LogInformation("Группа товаров ID = {Id} успешно найдена.", id);

            return _mapper.Map<ProductGroupDto>(productGroup);
        }

        public async Task<ProductGroupDto> CreateAsync(CreateProductGroupDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато создание группы товаров: Название = {Name}.", input.Name);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка создания группы товаров: название отсутствует.");
                UserFriendlyException.PRODUCT_GROUP_NAME_CAN_NOT_BE_EMPTY();
            }

            var existingGroup = await _productGroupRepository.GetByNameAsync(input.Name, ct);
            if (existingGroup != null)
            {
                _logger.LogError("Ошибка создания группы товаров: группа с названием '{Name}' уже существует.", input.Name);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_NAME_ALREADY_EXISTS(input.Name);
            }

            // Получаем товары для расчета общей стоимости
            var products = new List<Product>();
            var totalPrice = 0m;

            foreach (var productId in input.ProductIds.Distinct())
            {
                var product = await _productRepository.GetAsync(productId, ct);
                if (product != null)
                {
                    products.Add(product);
                    totalPrice += product.Price;
                }
                else
                {
                    _logger.LogWarning("Товар ID = {ProductId} не найден, пропускаем.", productId);
                }
            }

            if (!products.Any())
            {
                _logger.LogError("Ошибка создания группы товаров: необходимо указать хотя бы один товар.");
                UserFriendlyException.PRODUCT_GROUP_MUST_HAVE_AT_LEAST_ONE_PRODUCT();
            }

            var calculatedValues = CalculateGroupPrices(totalPrice, input.PriceWithDiscount, input.DiscountPercentage, input.DiscountedAmount);

            var productGroup = _mapper.Map<ProductGroup>(input);
            productGroup.CreatedAt = DateTime.UtcNow;

            // Применяем рассчитанные значения
            productGroup.PriceWithDiscount = calculatedValues.priceWithDiscount;
            productGroup.DiscountPercentage = calculatedValues.discountPercentage;
            productGroup.DiscountedAmount = calculatedValues.discountedAmount;

            // Добавляем товары в группу
            foreach (var product in products)
            {
                productGroup.ProductGroupProducts.Add(new ProductGroupProduct
                {
                    ProductGroupId = productGroup.Id,
                    ProductId = product.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _productGroupRepository.AddAsync(productGroup, ct);
            await _productGroupRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Группа товаров успешно создана: ID = {Id}.", productGroup.Id);

            return _mapper.Map<ProductGroupDto>(productGroup);
        }

        public async Task<ProductGroupDto> UpdateAsync(UpdateProductGroupDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато обновление группы товаров ID = {Id}.", input.Id);

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                _logger.LogError("Ошибка обновления группы товаров ID = {Id}: название отсутствует.", input.Id);
                UserFriendlyException.PRODUCT_GROUP_NAME_CAN_NOT_BE_EMPTY();
            }

            var productGroup = await _productGroupRepository.GetAsync(input.Id, ct);
            if (productGroup == null)
            {
                _logger.LogError("Ошибка обновления: группа товаров ID = {Id} не найдена.", input.Id);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            if (productGroup.Name != input.Name)
            {
                var existingGroup = await _productGroupRepository.GetByNameAsync(input.Name, ct);
                if (existingGroup != null && existingGroup.Id != input.Id)
                {
                    _logger.LogError(
                        "Ошибка обновления группы товаров ID = {Id}: группа с названием '{Name}' уже существует.",
                        input.Id, input.Name
                    );
                    UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_NAME_ALREADY_EXISTS(input.Name);
                }
            }

            // Получаем текущие товары или новые (если указаны)
            var products = new List<Product>();
            var totalPrice = 0m;

            if (input.ProductIds.Any())
            {
                // Используем новые товары
                foreach (var productId in input.ProductIds.Distinct())
                {
                    var product = await _productRepository.GetAsync(productId, ct);
                    if (product != null)
                    {
                        products.Add(product);
                        totalPrice += product.Price;
                    }
                    else
                    {
                        _logger.LogWarning("Товар ID = {ProductId} не найден, пропускаем.", productId);
                    }
                }
            }
            else
            {
                // Используем текущие товары
                foreach (var pgp in productGroup.ProductGroupProducts)
                {
                    var product = await _productRepository.GetAsync(pgp.ProductId, ct);
                    if (product != null)
                    {
                        products.Add(product);
                        totalPrice += product.Price;
                    }
                }
            }

            if (!products.Any())
            {
                _logger.LogError("Ошибка обновления группы товаров: группа должна содержать хотя бы один товар.");
                UserFriendlyException.PRODUCT_GROUP_MUST_HAVE_AT_LEAST_ONE_PRODUCT();
            }

            var calculatedValues = CalculateGroupPrices(totalPrice, input.PriceWithDiscount, input.DiscountPercentage, input.DiscountedAmount);

            _mapper.Map(input, productGroup);

            productGroup.PriceWithDiscount = calculatedValues.priceWithDiscount;
            productGroup.DiscountPercentage = calculatedValues.discountPercentage;
            productGroup.DiscountedAmount = calculatedValues.discountedAmount;

            // Обновление состава товаров (если указаны новые товары)
            if (input.ProductIds.Any())
            {
                // Очищаем текущие связи
                productGroup.ProductGroupProducts.Clear();

                // Добавляем новые связи
                foreach (var product in products)
                {
                    productGroup.ProductGroupProducts.Add(new ProductGroupProduct
                    {
                        ProductGroupId = productGroup.Id,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            _productGroupRepository.Update(productGroup);
            await _productGroupRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Группа товаров ID = {Id} обновлена успешно.", input.Id);

            return _mapper.Map<ProductGroupDto>(productGroup);
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление группы товаров ID = {Id}.", id);

            var productGroup = await _productGroupRepository.GetAsync(id, ct);
            if (productGroup == null)
            {
                _logger.LogError("Ошибка удаления: группа товаров ID = {Id} не найдена.", id);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(id);
            }

            _productGroupRepository.Delete(productGroup);
            await _productGroupRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Группа товаров ID = {Id} успешно удалена.", id);
        }

        public async Task AddProductToGroupAsync(long productGroupId, long productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато добавление товара ID = {ProductId} в группу ID = {GroupId}.", productId, productGroupId);

            var productGroup = await _productGroupRepository.GetAsync(productGroupId, ct);
            if (productGroup == null)
            {
                _logger.LogError("Ошибка добавления товара: группа товаров ID = {Id} не найдена.", productGroupId);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(productGroupId);
            }

            var product = await _productRepository.GetAsync(productId, ct);
            if (product == null)
            {
                _logger.LogError("Ошибка добавления товара: товар ID = {Id} не найден.", productId);
                UserFriendlyException.PRODUCT_WITH_SPECIFIED_ID_WAS_NOT_FOUND(productId);
            }

            // Проверяем, не добавлен ли уже товар
            if (productGroup.ProductGroupProducts.Any(p => p.ProductId == productId))
            {
                _logger.LogError(
                    "Ошибка добавления товара: товар ID = {ProductId} уже добавлен в группу ID = {GroupId}.",
                    productId, productGroupId
                );
                UserFriendlyException.PRODUCT_ALREADY_IN_GROUP(productId, productGroupId);
            }

            // Добавляем товар
            productGroup.ProductGroupProducts.Add(new ProductGroupProduct
            {
                ProductGroupId = productGroupId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            });

            // Пересчитываем цены после добавления товара
            await RecalculateGroupPricesAsync(productGroup, ct);

            _productGroupRepository.Update(productGroup);
            await _productGroupRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Товар ID = {ProductId} успешно добавлен в группу ID = {GroupId}.", productId, productGroupId);
        }

        public async Task RemoveProductFromGroupAsync(long productGroupId, long productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Начато удаление товара ID = {ProductId} из группы ID = {GroupId}.", productId, productGroupId);

            var productGroup = await _productGroupRepository.GetAsync(productGroupId, ct);
            if (productGroup == null)
            {
                _logger.LogError("Ошибка удаления товара: группа товаров ID = {Id} не найдена.", productGroupId);
                UserFriendlyException.PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(productGroupId);
            }

            var productGroupProduct = productGroup.ProductGroupProducts.FirstOrDefault(p => p.ProductId == productId);
            if (productGroupProduct == null)
            {
                _logger.LogError("Ошибка удаления товара: товар ID = {ProductId} не найден в группе ID = {GroupId}.", productId, productGroupId);
                UserFriendlyException.PRODUCT_NOT_IN_GROUP(productId, productGroupId);
            }

            productGroup.ProductGroupProducts.Remove(productGroupProduct);

            // Пересчитываем цены после удаления товара
            await RecalculateGroupPricesAsync(productGroup, ct);

            _productGroupRepository.Update(productGroup);
            await _productGroupRepository.UnitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Товар ID = {ProductId} успешно удален из группы ID = {GroupId}.", productId, productGroupId);
        }


        /// <summary>
        /// Рассчитать цены группы на основе различных входных параметров
        /// </summary>
        private (decimal priceWithDiscount, decimal discountPercentage, decimal discountedAmount) CalculateGroupPrices(decimal totalPrice, decimal priceWithDiscount, decimal discountPercentage, decimal discountedAmount)
        {
            var specifiedValues = new List<(bool isSpecified, string name)>
            {
                (priceWithDiscount > 0, "PriceWithDiscount"),
                (discountPercentage > 0, "DiscountPercentage"),
                (discountedAmount > 0, "DiscountedAmount")
            };

            var specifiedCount = specifiedValues.Count(x => x.isSpecified);

            if (specifiedCount == 0)
            {
                // Если ничего не указано - без скидки
                return (totalPrice, 0, 0);
            }

            if (specifiedCount > 1)
            {
                var specifiedNames = specifiedValues
                    .Where(x => x.isSpecified)
                    .Select(x => x.name)
                    .ToList();

                _logger.LogError("Указано несколько способов расчета скидки: {Names}. Должен быть указан только один параметр.", string.Join(", ", specifiedNames));
                UserFriendlyException.PRODUCT_GROUP_ONLY_ONE_DISCOUNT_METHOD_ALLOWED();
            }

            // Рассчитываем в зависимости от указанного параметра
            if (priceWithDiscount > 0)
            {
                if (priceWithDiscount > totalPrice)
                {
                    _logger.LogError("Цена со скидкой ({PriceWithDiscount}) не может быть больше общей стоимости ({TotalPrice}).", priceWithDiscount, totalPrice);
                    UserFriendlyException.PRODUCT_GROUP_PRICE_WITH_DISCOUNT_CANNOT_BE_GREATER_THAN_TOTAL();
                }

                discountPercentage = ProductGroupCalculator.CalculateDiscountPercentage(totalPrice, priceWithDiscount);
                discountedAmount = totalPrice - priceWithDiscount;

                return (priceWithDiscount, discountPercentage, discountedAmount);
            }
            else if (discountPercentage > 0)
            {
                if (discountPercentage > 100)
                {
                    _logger.LogError("Процент скидки ({DiscountPercentage}) не может превышать 100%.", discountPercentage);
                    UserFriendlyException.PRODUCT_GROUP_DISCOUNT_PERCENTAGE_CANNOT_EXCEED_100();
                }

                priceWithDiscount = ProductGroupCalculator.CalculatePriceWithDiscount(totalPrice, discountPercentage);
                discountedAmount = ProductGroupCalculator.CalculateDiscountedAmount(totalPrice, discountPercentage);

                return (priceWithDiscount, discountPercentage, discountedAmount);
            }
            else if (discountedAmount > 0)
            {
                if (discountedAmount > totalPrice)
                {
                    _logger.LogError("Сумма скидки ({DiscountedAmount}) не может быть больше общей стоимости ({TotalPrice}).", discountedAmount, totalPrice);
                    UserFriendlyException.PRODUCT_GROUP_DISCOUNTED_AMOUNT_CANNOT_BE_GREATER_THAN_TOTAL();
                }

                discountPercentage = ProductGroupCalculator.CalculateDiscountPercentageFromAmount(totalPrice, discountedAmount);
                priceWithDiscount = totalPrice - discountedAmount;

                return (priceWithDiscount, discountPercentage, discountedAmount);
            }

            return (totalPrice, 0, 0);
        }

        /// <summary>
        /// Пересчитать цены группы на основе текущего состава товаров
        /// </summary>
        private async Task RecalculateGroupPricesAsync(ProductGroup productGroup, CancellationToken ct = default)
        {
            // Рассчитываем общую стоимость текущих товаров
            var totalPrice = 0m;
            foreach (var pgp in productGroup.ProductGroupProducts)
            {
                var product = await _productRepository.GetAsync(pgp.ProductId, ct);
                if (product != null)
                {
                    totalPrice += product.Price;
                }
            }

            if (totalPrice <= 0)
            {
                productGroup.PriceWithDiscount = 0;
                productGroup.DiscountPercentage = 0;
                productGroup.DiscountedAmount = 0;
                return;
            }

            // Сохраняем текущий процент скидки (или рассчитываем заново)
            if (productGroup.DiscountPercentage > 0)
            {
                // Сохраняем процент скидки
                var calculated = CalculateGroupPrices(totalPrice, 0, productGroup.DiscountPercentage, 0);

                productGroup.PriceWithDiscount = calculated.priceWithDiscount;
                productGroup.DiscountedAmount = calculated.discountedAmount;
            }
            else
            {
                // Без скидки
                productGroup.PriceWithDiscount = totalPrice;
                productGroup.DiscountPercentage = 0;
                productGroup.DiscountedAmount = 0;
            }
        }
    }
}