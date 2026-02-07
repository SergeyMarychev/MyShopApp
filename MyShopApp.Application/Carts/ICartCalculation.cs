using MyShopApp.Domain.Products;

namespace MyShopApp.Application.Carts
{
    public interface ICartCalculation
    {
        /// <summary>
        /// Вычисление суммы стоимости двух товаров
        /// </summary>
        /// <param name="product1">Первый товар или null</param>
        /// <param name="product2">Второй товар или null</param>
        /// <returns>Сумма цен двух товаров. Если товар равен null, его цена считается как 0</returns>
        decimal Sum(Product? product1, Product? product2);
    }
}
