using MyShopApp.Application.Carts;
using MyShopApp.Domain.Products;

namespace MyShopApp.Tests.Application
{
    public class CartCalculationTests
    {
        [Fact(DisplayName = "Суммирование двух продуктов выполняется корректно.")]
        public void Sum_Correct()
        {
            // Arrange
            var cartCalculation = new CartCalculation();

            var product1 = new Product
            {
                Name = "Джинсы",
                Price = 4000,
            };
            var product2 = new Product
            {
                Name = "Футболка",
                Price = 2000,
            };

            // Act
            var sum = cartCalculation.Sum(product1, product2);

            // Assert
            Assert.Equal(6000, sum);
        }

        [Fact(DisplayName = "Суммирование двух продуктов выполняется корректно с null")]
        public void Sum_Null()
        {
            // Arrange
            var cartCalculation = new CartCalculation();

            var product1 = new Product
            {
                Name = "Джинсы",
                Price = 4000,
            };

            // Act
            var sum1 = cartCalculation.Sum(product1, null);
            var sum2 = cartCalculation.Sum(null, null);
            var sum3 = cartCalculation.Sum(null, product1);

            // Assert
            Assert.Equal(4000, sum1);
            Assert.Equal(0, sum2);
            Assert.Equal(4000, sum3);
        }
    }
}
