namespace MyShopApp.Application.Contracts.Carts
{
    public class CartCalculationResult
    {
        public int ProductCount { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal ProductTotalPriceWithDiscount { get; set; }
    }
}
