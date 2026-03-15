using MyShopApp.Application.Contracts.Authorization.Dto;

namespace MyShopApp.Application.Authorization
{
    public interface IAccountService
    {
        Task<LoginResultDto> RequestCodeAsync(string phoneNumber, CancellationToken ct = default);
        Task<TokenDto> VerifyCodeAsync(VerifySmsCodeDto input, CancellationToken ct = default);
    }
}
