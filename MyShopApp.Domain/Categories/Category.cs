using MyShopApp.Domain.Common;
using MyShopApp.Domain.ParentCategories;

namespace MyShopApp.Domain.Categories
{
    public sealed class Category : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; } = string.Empty;
        public long? ParentCategoryId { get; set; }
        public ParentCategory? ParentCategory { get; set; }
    }
}
