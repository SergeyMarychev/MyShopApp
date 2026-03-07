using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.ParentCategories.Dto
{
    public sealed class CreateParentCategoryDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
