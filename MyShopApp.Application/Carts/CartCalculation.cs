using MyShopApp.Domain.Products;

namespace MyShopApp.Application.Carts
{
    public class CartCalculation : ICartCalculation
    {
        public decimal Sum(Product? product1, Product? product2)
        {
            return (product1?.Price ?? 0m) + (product2?.Price ?? 0m);
        }
    }
}
