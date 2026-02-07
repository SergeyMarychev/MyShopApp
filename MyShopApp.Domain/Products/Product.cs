using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.ProductGroups;

namespace MyShopApp.Domain.Products
{
    public sealed class Product : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; } = string.Empty;

        // Навигационное свойство через промежуточную таблицу
        public ICollection<ProductGroupProduct> ProductGroupProducts { get; set; } = new List<ProductGroupProduct>();
    }
}
