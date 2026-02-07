using MyShopApp.Domain.Common;
using MyShopApp.Domain.Products;

namespace MyShopApp.Domain.ProductGroups
{
    public sealed class ProductGroupProduct : Entity, IHasCreationTime
    {
        public long ProductGroupId { get; set; }
        public ProductGroup ProductGroup { get; set; } = null!;
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
