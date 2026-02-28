using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Users;
using System.Security.Claims;

namespace MyShopApp.WebApi.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
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
        public async Task<IActionResult> GetUser(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await _userAppService.GetUserAsync(userId, ct);
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto input, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            if (input.Id != userId)
            {
                return Forbid();
            }

            await _userAppService.UpdateUserAsync(input, ct);
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAccount(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _userAppService.DeleteAccountAsync(userId, ct);
            return Ok();
        }
    }
}
