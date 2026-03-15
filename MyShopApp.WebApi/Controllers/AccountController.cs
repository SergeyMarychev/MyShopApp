using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Authorization;
using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Domain.Users;

namespace MyShopApp.WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Авторизация - запрос кода подтверждения
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(string phoneNumber, CancellationToken ct)
        {
            var result = await _accountService.RequestCodeAsync(phoneNumber, ct);

            if (result.RequiresCooldown)
            {
                return BadRequest(new
                {
                    error = $"Подождите {result.CooldownSeconds} секунд перед повторным запросом",
                    cooldownSeconds = result.CooldownSeconds
                });
            }

            return Ok(phoneNumber);
        }

        /// <summary>
        /// Проверка кода и получение токена
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyCode(VerifySmsCodeDto input, CancellationToken ct)
        {
            var token = await _accountService.VerifyCodeAsync(input, ct);
            return Ok(token);
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Выход из системы");
            return Ok();
        }
    }
}