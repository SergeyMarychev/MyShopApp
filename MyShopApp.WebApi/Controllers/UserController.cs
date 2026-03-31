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

        /// <summary>
        /// Получить идентификатор текущего авторизованного пользователя
        /// </summary>
        /// <returns>Идентификатор пользователя</returns>
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException();
            }
            return long.Parse(userIdClaim);
        }

        /// <summary>
        /// Получить профиль текущего пользователя
        /// </summary>
        /// <returns>Данные профиля пользователя</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await _userAppService.GetAsync(userId, ct);
            return Ok(result);
        }

        /// <summary>
        /// Обновить профиль пользователя
        /// </summary>
        /// <returns>Результат обновления профиля</returns>
        /// <remarks>Если указан новый email, на него будет отправлен код подтверждения. Email обновится только после вызова ConfirmEmail</remarks>
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

        /// <summary>
        /// Удалить аккаунт пользователя
        /// </summary>
        /// <returns>Результат удаления аккаунта</returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _userAppService.DeleteAsync(userId, ct);
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Аккаунт успешно удален" });
        }

        /// <summary>
        /// Подтвердить email пользователя
        /// </summary>
        /// <returns>Результат подтверждения email</returns>
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