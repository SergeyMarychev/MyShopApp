using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Categories
{
    public sealed class Category : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
