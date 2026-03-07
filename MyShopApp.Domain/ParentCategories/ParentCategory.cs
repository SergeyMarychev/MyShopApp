using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.ParentCategories
{
    public sealed class ParentCategory : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
