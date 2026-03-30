using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.Users;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    internal sealed class AddressRepository : EfRepositoryBase, IAddressRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Address> _table => _context.Addresses;

        public AddressRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<Address> GetLastAddressByUserIdAsync(long userId, CancellationToken ct = default)
        {
            return await _table
                .Where(a => a.UserAddresses.Any(ua => ua.UserId == userId) && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }
    }
}
