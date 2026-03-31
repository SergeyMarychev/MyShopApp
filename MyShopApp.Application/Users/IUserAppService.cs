using MyShopApp.Application.Contracts.Email;
using MyShopApp.Application.Contracts.Users;

namespace MyShopApp.Application.Users
{
    public interface IUserAppService
    {
        Task<UserDto> GetAsync(long userId, CancellationToken ct = default);
        Task UpdateAsync(UpdateUserDto input, CancellationToken ct = default);
        Task DeleteAsync(long userId, CancellationToken ct = default);
        Task<ConfirmEmailResultDto> ConfirmEmailAsync(long userId, ConfirmEmailDto input, CancellationToken ct = default);
    }
}
