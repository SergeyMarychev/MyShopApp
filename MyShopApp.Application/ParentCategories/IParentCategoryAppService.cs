using MyShopApp.Application.Contracts.ParentCategories.Dto;

namespace MyShopApp.Application.ParentCategories
{
    public interface IParentCategoryAppService
    {
        Task<IEnumerable<ParentCategoryDto>> GetAllAsync(CancellationToken ct = default);
        Task<ParentCategoryDto> GetAsync(long id, CancellationToken ct = default);
        Task<ParentCategoryDto> CreateAsync(CreateParentCategoryDto input, CancellationToken ct = default);
        Task<ParentCategoryDto> UpdateAsync(UpdateParentCategoryDto input, CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);
        Task AddCategoryAsync(AddCategoryToParentDto input, CancellationToken ct = default);
        Task RemoveCategoryAsync(long parentCategoryId, long categoryId, CancellationToken ct = default);
    }
}
