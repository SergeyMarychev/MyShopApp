using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public interface IUserRepository : IRepository
    {
        Task<User?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false, CancellationToken ct = default);
        Task<User?> GetByPhoneNumberIncludeDeletedAsync(string phoneNumber, CancellationToken ct = default);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        void Update(User user);
        void Delete(User user);
    }
}
