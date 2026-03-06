using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.ParentCategories.Dto
{
    public sealed class UpdateParentCategoryDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
