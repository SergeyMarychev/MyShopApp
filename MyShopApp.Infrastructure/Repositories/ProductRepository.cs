using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.Products;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    public sealed class ProductRepository : EfRepositoryBase, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Product> _table => _context.Products;

        public ProductRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
        {
            return await _table
                .Include(p => p.Category) 
                .ToListAsync(ct);
        }

        public async Task<Product?> GetAsync(long id, CancellationToken ct = default)
        {
            return await _table
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _table
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct = default)
        {
            await _table.AddAsync(product, ct);
        }

        public void Update(Product product)
        {
            _table.Update(product);
        }

        public void Delete(Product product)
        {
            _table.Remove(product);
        }
    }
}
