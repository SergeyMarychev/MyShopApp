using MyShopApp.Application.Contracts.Categories.Dto;
using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.ParentCategories.Dto
{
    public sealed class ParentCategoryDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
