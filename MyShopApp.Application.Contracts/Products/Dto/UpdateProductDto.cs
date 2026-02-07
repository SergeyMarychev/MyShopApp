using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.Products.Dto
{
    public sealed class UpdateProductDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
    }
}
