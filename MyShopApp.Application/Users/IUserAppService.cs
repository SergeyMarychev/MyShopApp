using MyShopApp.Application.Contracts.Users;

namespace MyShopApp.Application.Users
{
    public interface IUserAppService
    {
        Task<UserDto> GetUserAsync(long userId, CancellationToken ct = default);
        Task UpdateUserAsync(UpdateUserDto input, CancellationToken ct = default);
        Task DeleteAccountAsync(long userId, CancellationToken ct = default);
        Task<bool> RestoreAccountAsync(string phoneNumber, CancellationToken ct = default);
    }
}
