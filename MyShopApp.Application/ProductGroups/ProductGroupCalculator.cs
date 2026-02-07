namespace MyShopApp.Application.ProductGroups
{
    public static class ProductGroupCalculator
    {
        /// <summary>
        /// Рассчитать сумму всех товаров в группе
        /// </summary>
        public static decimal CalculateTotalPrice(IEnumerable<decimal> productPrices)
        {
            return productPrices.Sum();
        }

        /// <summary>
        /// Рассчитать цену со скидкой на основе процента
        /// </summary>
        public static decimal CalculatePriceWithDiscount(decimal totalPrice, decimal discountPercentage)
        {
            if (discountPercentage < 0) discountPercentage = 0;
            if (discountPercentage > 100) discountPercentage = 100;

            return totalPrice * (100 - discountPercentage) / 100;
        }

        /// <summary>
        /// Рассчитать сумму скидки на основе процента
        /// </summary>
        public static decimal CalculateDiscountedAmount(decimal totalPrice, decimal discountPercentage)
        {
            if (discountPercentage < 0) discountPercentage = 0;
            if (discountPercentage > 100) discountPercentage = 100;

            return totalPrice * discountPercentage / 100;
        }

        /// <summary>
        /// Рассчитать процент скидки на основе цены со скидкой
        /// </summary>
        public static decimal CalculateDiscountPercentage(decimal totalPrice, decimal priceWithDiscount)
        {
            if (totalPrice <= 0) return 0;
            if (priceWithDiscount >= totalPrice) return 0;

            return (totalPrice - priceWithDiscount) / totalPrice * 100;
        }

        /// <summary>
        /// Рассчитать процент скидки на основе суммы скидки
        /// </summary>
        public static decimal CalculateDiscountPercentageFromAmount(decimal totalPrice, decimal discountedAmount)
        {
            if (totalPrice <= 0) return 0;
            if (discountedAmount <= 0) return 0;
            if (discountedAmount >= totalPrice) return 100;

            return discountedAmount / totalPrice * 100;
        }

        /// <summary>
        /// Проверяет корректность введенных данных
        /// </summary>
        public static bool ValidatePrices(decimal totalPrice, decimal? priceWithDiscount, decimal? discountPercentage, decimal? discountedAmount)
        {
            if (totalPrice <= 0) return false;

            // Проверяем, что указан только один способ расчета
            var specifiedValues = new[]
            {
                priceWithDiscount.HasValue && priceWithDiscount > 0,
                discountPercentage.HasValue && discountPercentage > 0,
                discountedAmount.HasValue && discountedAmount > 0
            };

            return specifiedValues.Count(x => x) == 1;
        }
    }
}