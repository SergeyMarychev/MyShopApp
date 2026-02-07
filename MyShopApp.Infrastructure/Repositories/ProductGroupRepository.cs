using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.ProductGroups;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    public sealed class ProductGroupRepository : EfRepositoryBase, IProductGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<ProductGroup> _table => _context.ProductGroups;

        public ProductGroupRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductGroup>> GetAllAsync(CancellationToken ct = default)
        {
            return await _table
                .Include(pg => pg.ProductGroupProducts)
                    .ThenInclude(pgp => pgp.Product) // Включаем товары через промежуточную таблицу
                .ToListAsync(ct);
        }

        public async Task<ProductGroup?> GetAsync(long id, CancellationToken ct = default)
        {
            return await _table
                .Include(pg => pg.ProductGroupProducts)
                    .ThenInclude(pgp => pgp.Product) // Включаем товары через промежуточную таблицу
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<ProductGroup?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _table
                .Include(pg => pg.ProductGroupProducts)
                    .ThenInclude(pgp => pgp.Product) // Включаем товары через промежуточную таблицу
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
        }

        public async Task AddAsync(ProductGroup productGroup, CancellationToken ct = default)
        {
            await _table.AddAsync(productGroup, ct);
        }

        public void Update(ProductGroup productGroup)
        {
            _table.Update(productGroup);
        }

        public void Delete(ProductGroup productGroup)
        {
            _table.Remove(productGroup);
        }
    }
}