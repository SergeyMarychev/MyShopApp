using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.Users;
using MyShopApp.Infrastructure.Common;

namespace MyShopApp.Infrastructure.Repositories
{
    internal sealed class UserRepository : EfRepositoryBase, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<User> _table => _context.Users;

        public UserRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _table.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            return await query.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, ct);
        }

        public async Task<User?> GetByPhoneNumberIncludeDeletedAsync(string phoneNumber, CancellationToken ct = default)
        {
            return await _table.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, ct);
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            return await _table.AnyAsync(x => x.PhoneNumber == phoneNumber && !x.IsDeleted, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _table.AddAsync(user, ct);
        }

        public void Update(User user)
        {
            _table.Update(user);
        }

        public void Delete(User user)
        {
            _table.Remove(user);
        }
    }
}
