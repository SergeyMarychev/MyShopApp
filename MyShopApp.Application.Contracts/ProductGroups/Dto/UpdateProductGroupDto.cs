using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.ProductGroups.Dto
{
    public sealed class UpdateProductGroupDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal PriceWithDiscount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedAmount { get; set; }
        public string Image { get; set; } = string.Empty;
        public IReadOnlyList<long> ProductIds { get; set; } = [];
    }
}
