namespace MyShopApp.Application.Contracts.ParentCategories.Dto
{
    public sealed class AddCategoryToParentDto
    {
        public long ParentCategoryId { get; set; }
        public long CategoryId { get; set; }
    }
}
