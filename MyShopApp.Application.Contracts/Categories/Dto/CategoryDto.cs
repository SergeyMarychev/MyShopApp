using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.Categories.Dto
{
    public sealed class CategoryDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
