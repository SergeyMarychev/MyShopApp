using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Products
{
    public sealed class Product : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
