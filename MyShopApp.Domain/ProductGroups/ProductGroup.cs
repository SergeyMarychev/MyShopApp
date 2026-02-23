using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.ProductGroups
{
    public sealed class ProductGroup : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public decimal PriceWithDiscount { get; set; }
        public decimal DiscountPercentage { get; set; } // процент скидки 
        public decimal DiscountedAmount { get; set; } // сумма скидки
        public string Image {  get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProductGroupProduct> ProductGroupProducts { get; set; } = new List<ProductGroupProduct>();
    }
}
