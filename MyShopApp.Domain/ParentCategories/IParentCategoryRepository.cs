using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.ParentCategories
{
    public interface IParentCategoryRepository : IRepository
    {
        Task<IEnumerable<ParentCategory>> GetAllAsync(CancellationToken ct = default);
        Task<ParentCategory?> GetAsync(long id, CancellationToken ct = default);
        Task<ParentCategory?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<ParentCategory?> GetWithCategoriesAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<ParentCategory>> GetAllWithCategoriesAsync(CancellationToken ct = default);
        Task AddAsync(ParentCategory parentCategory, CancellationToken ct = default);
        void Update(ParentCategory parentCategory);
        void Delete(ParentCategory parentCategory);
    }
}
