using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.Email;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Users;
using MyShopApp.Domain.Users;
using System.Security.Claims;

namespace MyShopApp.WebApi.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IUserAppService _userAppService;
        private readonly SignInManager<User> _signInManager;

        public UserController(IUserAppService userAppService, SignInManager<User> signInManager)
        {
            _userAppService = userAppService;
            _signInManager = signInManager;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException();
            }
            return long.Parse(userIdClaim);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await _userAppService.GetAsync(userId, ct);
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto input, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            if (input.Id != userId)
            {
                return Forbid();
            }

            await _userAppService.UpdateAsync(input, ct);
            return Ok(new { message = "Профиль обновлен. Если вы изменили email, подтвердите его кодом." });
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _userAppService.DeleteAsync(userId, ct);
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Аккаунт успешно удален" });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto input, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await _userAppService.ConfirmEmailAsync(userId, input, ct);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}