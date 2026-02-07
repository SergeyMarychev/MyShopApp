using MyShopApp.Application.Contracts.Common.Dto;
using System.ComponentModel;

namespace MyShopApp.Application.Contracts.Products.Dto
{
    public sealed class ProductDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
