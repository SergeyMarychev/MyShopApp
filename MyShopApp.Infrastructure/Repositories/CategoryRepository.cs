using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Common;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    internal sealed class CategoryRepository : EfRepositoryBase, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Category> _table => _context.Categories;

        public CategoryRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        {
            return await _table.ToListAsync(ct);
        }

        public async Task<Category?> GetAsync(long id, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct = default)
        {
            await _table.AddAsync(category, ct);
        }

        public void Update(Category category)
        {
            _table.Update(category);
        }

        public void Delete(Category category)
        {
            _table.Remove(category);
        }
    }
}
