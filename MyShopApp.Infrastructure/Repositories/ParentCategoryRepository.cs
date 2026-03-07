using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.ParentCategories;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    internal sealed class ParentCategoryRepository : EfRepositoryBase, IParentCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<ParentCategory> _table => _context.ParentCategories;

        public ParentCategoryRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParentCategory>> GetAllAsync(CancellationToken ct = default)
        {
            return await _table.ToListAsync(ct);
        }

        public async Task<ParentCategory?> GetAsync(long id, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<ParentCategory?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
        }
        public async Task AddAsync(ParentCategory parentCategory, CancellationToken ct = default)
        {
            await _table.AddAsync(parentCategory, ct);
        }

        public void Update(ParentCategory parentCategory)
        {
            _table.Update(parentCategory);
        }

        public void Delete(ParentCategory parentCategory)
        {
            _table.Remove(parentCategory);
        }
    }
}