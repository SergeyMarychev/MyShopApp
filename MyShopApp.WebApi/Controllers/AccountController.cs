using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Domain.Users;
using MyShopApp.WebApi.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyShopApp.WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;

        // Простое хранилище кодов в памяти (для имитации)
        private static readonly Dictionary<string, (string Code, DateTime Expiry)> _codeStorage = new();

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto input, CancellationToken ct)
        {
            _logger.LogInformation("Запрос кода для номера: {PhoneNumber}", input.PhoneNumber);

            if (string.IsNullOrWhiteSpace(input.PhoneNumber))
            {
                return BadRequest(new { errorText = "Номер телефона не может быть пустым." });
            }

            // Генерируем код
            var code = GenerateCode();

            // Сохраняем код с временем жизни 5 минут
            _codeStorage[input.PhoneNumber] = (code, DateTime.UtcNow.AddMinutes(5));

            // В консоль выводим код для имитации
            _logger.LogInformation("Для номера {PhoneNumber} сгенерирован код: {Code}", input.PhoneNumber, code);

            // Проверяем существует ли пользователь (включая удаленных)
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == input.PhoneNumber, ct);

            var isNewUser = user == null;
            var isDeletedUser = user != null && user.IsDeleted;

            var response = new LoginResponseDto
            {
                PhoneNumber = input.PhoneNumber,
                IsNewUser = isNewUser,
                IsDeletedUser = isDeletedUser
            };

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VerifySmsCode([FromBody] VerifySmsCodeDto input, CancellationToken ct)
        {
            _logger.LogInformation("Проверка кода для номера: {PhoneNumber}", input.PhoneNumber);

            // Проверяем код
            if (!_codeStorage.TryGetValue(input.PhoneNumber, out var codeData))
            {
                return BadRequest(new { errorText = "Код не найден. Запросите новый код." });
            }

            if (codeData.Expiry < DateTime.UtcNow)
            {
                _codeStorage.Remove(input.PhoneNumber);
                return BadRequest(new { errorText = "Код истек. Запросите новый код." });
            }

            if (codeData.Code != input.Code)
            {
                return BadRequest(new { errorText = "Неверный код подтверждения." });
            }

            // Код верный, удаляем его из хранилища
            _codeStorage.Remove(input.PhoneNumber);

            // Ищем пользователя ВКЛЮЧАЯ УДАЛЕННЫХ
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == input.PhoneNumber, ct);

            var isNewUser = false;
            var isRestored = false;

            if (user == null)
            {
                isNewUser = true;

                user = new User
                {
                    PhoneNumber = input.PhoneNumber,
                    UserName = input.PhoneNumber, // просто номер телефона для отображения как имени
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Ошибка создания пользователя: {Errors}", errors);
                    return BadRequest(new { errorText = $"Ошибка создания пользователя: {errors}" });
                }

                _logger.LogInformation("Создан новый пользователь с ID: {UserId}", user.Id);
            }
            else if (user.IsDeleted)
            {
                // Проверяем, можно ли восстановить аккаунт (30 дней)
                if (user.CanBeRestored)
                {
                    // Восстанавливаем аккаунт
                    user.IsDeleted = false;
                    user.DeletedAt = null;

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        _logger.LogError("Ошибка восстановления аккаунта: {Errors}", errors);
                        return BadRequest(new { errorText = "Ошибка восстановления аккаунта." });
                    }

                    isRestored = true;
                    _logger.LogInformation("Аккаунт восстановлен для номера: {PhoneNumber}, ID: {UserId}",
                        input.PhoneNumber, user.Id);
                }
                else
                {
                    // Аккаунт удален более 30 дней назад
                    _logger.LogWarning("Попытка входа в удаленный >30 дней аккаунт: {PhoneNumber}", input.PhoneNumber);
                    return BadRequest(new { errorText = "Аккаунт был удален более 30 дней назад. Создайте новый аккаунт." });
                }
            }

            // Получаем identity для создания токена
            var identity = await GetIdentityAsync(user);

            // создаем JWT-токен
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: identity.Claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new LoginResponseDto
            {
                Token = encodedJwt,
                Username = user.UserName,
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                IsNewUser = isNewUser,
                IsRestored = isRestored
            };

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Выход из системы");
            return Ok();
        }

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}