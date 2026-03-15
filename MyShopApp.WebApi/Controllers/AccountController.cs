using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyShopApp.Application.Authorization;
using MyShopApp.Application.Authorization.Models;
using MyShopApp.Application.Authorization.Settings;
using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Users;
using MyShopApp.WebApi.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace MyShopApp.WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly SignInManager<User> _signInManager;      // Для управления входом/выходом пользователя
        private readonly UserManager<User> _userManager;          // Для управления пользователями (создание, обновление)
        private readonly ILogger<AccountController> _logger;      // Для логирования
        private readonly SmsCodeSettings _smsSettings;            // Настройки для SMS кодов из appsettings.json
        private readonly IDistributedCache _cache;                // Распределенный кэш (в памяти или Redis)
        private readonly AccountRecoveryService _recoveryService;

        // Префикс для ключей в кэше, чтобы избежать конфликтов с другими данными
        private const string CACHE_KEY_PREFIX = "sms_code_";

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<AccountController> logger,
            IOptions<SmsCodeSettings> smsSettings,
            IDistributedCache cache,
            AccountRecoveryService recoveryService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _smsSettings = smsSettings.Value;   // Получаем значения настроек из IOptions
            _cache = cache;
            _recoveryService = recoveryService;
        }

        /// <summary>
        /// Формирует ключ для кэша на основе номера телефона
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <returns>Ключ для кэша с префиксом</returns>
        private string GetCacheKey(string phoneNumber) => $"{CACHE_KEY_PREFIX}{phoneNumber}";

        /// <summary>
        /// Получает данные SMS кода из кэша по номеру телефона
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Данные кода или null, если не найден</returns>
        private async Task<SmsCodeData?> GetCodeDataAsync(string phoneNumber, CancellationToken ct = default)
        {
            var cacheKey = GetCacheKey(phoneNumber);
            var cachedData = await _cache.GetStringAsync(cacheKey, ct);

            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            // Десериализуем JSON обратно в объект SmsCodeData
            return JsonSerializer.Deserialize<SmsCodeData>(cachedData);
        }

        /// <summary>
        /// Сохраняет данные SMS кода в кэш
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="data">Данные кода</param>
        /// <param name="ct">Токен отмены</param>
        private async Task SetCodeDataAsync(string phoneNumber, SmsCodeData data, CancellationToken ct = default)
        {
            var cacheKey = GetCacheKey(phoneNumber);
            var serialized = JsonSerializer.Serialize(data);

            var options = new DistributedCacheEntryOptions
            {
                // Время жизни в кэше = времени жизни кода из настроек
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_smsSettings.CodeLifetimeMinutes)
            };

            await _cache.SetStringAsync(cacheKey, serialized, options, ct);
        }

        /// <summary>
        /// Удаляет данные SMS кода из кэша
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="ct">Токен отмены</param>
        private async Task RemoveCodeDataAsync(string phoneNumber, CancellationToken ct = default)
        {
            var cacheKey = GetCacheKey(phoneNumber);
            await _cache.RemoveAsync(cacheKey, ct);
        }

        /// <summary>
        /// 1. Проверяет существование пользователя в БД:
        ///    - Если пользователя нет → создает нового, выдает код
        ///    - Если пользователь есть, но удален → проверяет возможность восстановления (30 дней)
        ///    - Если пользователь есть и активен → просто выдает код
        /// 2. Проверяет кулдаун (нельзя запрашивать код в течение 1 минуты)
        /// 3. Генерирует и сохраняет новый код в кэш
        /// </summary>
        /// <param name="phoneNumber">Номер телефона из URL</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о запросе кода</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(string phoneNumber, CancellationToken ct)
        {
            _logger.LogInformation("Запрос кода для номера: {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                UserFriendlyException.PHONE_NUMBER_CAN_NOT_BE_EMPTY();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);

            var isRestoreMode = user != null && user.IsDeleted;

            if (user == null)
            {
                // Создаем нового пользователя
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    UserName = phoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Ошибка создания пользователя: {Errors}", errors);
                    UserFriendlyException.USER_CREATION_FAILED(errors);
                }

                _logger.LogInformation("Создан новый пользователь с ID: {UserId}", user.Id);
            }
            else if (user.IsDeleted)
            {
                _logger.LogInformation("Пользователь с номером {PhoneNumber} удален. Будет восстановлен после проверки кода.", phoneNumber);
            }

            // 2. Проверяем кулдаун через кэш
            var existingData = await GetCodeDataAsync(phoneNumber, ct);
            if (existingData != null)
            {
                if (existingData.CooldownUntil.HasValue && existingData.CooldownUntil > DateTime.UtcNow)
                {
                    var secondsLeft = (int)(existingData.CooldownUntil.Value - DateTime.UtcNow).TotalSeconds;
                    _logger.LogWarning("Кулдаун для номера {PhoneNumber}: {SecondsLeft} секунд", phoneNumber, secondsLeft);

                    return Ok(new
                    {
                        phoneNumber,
                        cooldownSeconds = secondsLeft,
                        requiresCooldown = true
                    });
                }

                if (existingData.Expiry < DateTime.UtcNow || existingData.Attempts >= _smsSettings.MaxAttempts)
                {
                    await RemoveCodeDataAsync(phoneNumber, ct);
                }
            }

            // 3. Генерируем код
            var code = GenerateCode();

            // 4. Сохраняем код в кэш
            var newData = new SmsCodeData
            {
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(_smsSettings.CodeLifetimeMinutes),
                Attempts = 0,
                CooldownUntil = DateTime.UtcNow.AddSeconds(_smsSettings.CooldownSeconds),
                PhoneNumber = phoneNumber,
                IsRestoreMode = isRestoreMode,
            };

            await SetCodeDataAsync(phoneNumber, newData, ct);

            _logger.LogInformation("Для номера {PhoneNumber} сгенерирован код: {Code}, режим: {Mode} ", phoneNumber, code, isRestoreMode ? "восстановление" : "вход");

            return Ok(phoneNumber);
        }

        /// <summary>
        /// 1. Получает данные кода из кэша
        /// 2. Проверяет срок действия и количество попыток
        /// 3. При успехе генерирует JWT токен
        /// </summary>
        /// <param name="input">DTO с номером телефона и кодом</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>JWT токен и данные пользователя</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> VerifySmsCode(VerifySmsCodeDto input, CancellationToken ct)
        {
            _logger.LogInformation("Проверка кода для номера: {PhoneNumber}", input.PhoneNumber);

            var codeData = await GetCodeDataAsync(input.PhoneNumber, ct);

            if (codeData == null)
            {
                UserFriendlyException.CODE_NOT_FOUND();
            }

            if (codeData.Attempts >= _smsSettings.MaxAttempts)
            {
                await RemoveCodeDataAsync(input.PhoneNumber, ct);
                UserFriendlyException.MAX_ATTEMPTS_EXCEEDED();
            }

            if (codeData.Expiry < DateTime.UtcNow)
            {
                await RemoveCodeDataAsync(input.PhoneNumber, ct);
                UserFriendlyException.CODE_EXPIRED();
            }

            codeData.Attempts++;
            await SetCodeDataAsync(input.PhoneNumber, codeData, ct);

            if (codeData.Code != input.Code)
            {
                UserFriendlyException.INVALID_CODE(codeData.Attempts, _smsSettings.MaxAttempts);
            }

            // Код верный - удаляем из кэша
            await RemoveCodeDataAsync(input.PhoneNumber, ct);

            // Ищем пользователя
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == input.PhoneNumber, ct);

            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(0);
            }

            // Если это режим восстановления - восстанавливаем аккаунт
            if (codeData.IsRestoreMode && user.IsDeleted)
            {
                // Используем сервис для проверки
                if (_recoveryService.CanBeRestored(user.DeletedAt))
                {
                    user.IsDeleted = false;
                    user.DeletedAt = null;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Аккаунт восстановлен для номера: {PhoneNumber}", input.PhoneNumber);
                }
                else
                {
                    _logger.LogInformation("Срок восстановления истек. Создаем новый аккаунт для номера: {PhoneNumber}", input.PhoneNumber);

                    var newUser = new User
                    {
                        PhoneNumber = input.PhoneNumber,
                        UserName = input.PhoneNumber,
                        CreatedAt = DateTime.UtcNow,
                    };

                    var createReasult = await _userManager.CreateAsync(newUser);
                    if (!createReasult.Succeeded)
                    {
                        var errors = string.Join(", ", createReasult.Errors.Select(e => e.Description));
                        _logger.LogError("Ошибка создания нового пользователя: {Errors}", errors);
                        UserFriendlyException.USER_CREATION_FAILED(errors);
                    }

                    user = newUser;
                    _logger.LogInformation("Создан новый пользователь (Старый удален >30 дней) с Id: {UserId}", user.Id);
                }
            }

            // Генерируем JWT токен
            var tokenResponse = await GenerateJwtTokenAsync(user);

            return Ok(new
            {
                accessToken = tokenResponse.Token,
                expiresIn = tokenResponse.ExpiresIn
            });
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <returns>OK</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Выход из системы");
            return Ok();
        }

        /// <summary>
        /// Генерирует случайный 6-значный код подтверждения
        /// </summary>
        /// <returns>6-значный код в виде строки</returns>
        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Генерирует JWT токен для авторизованного пользователя
        /// </summary>
        /// <param name="user">Объект пользователя</param>
        /// <returns>JWT токен и время его жизни в секундах</returns>
        private async Task<(string Token, int ExpiresIn)> GenerateJwtTokenAsync(User user)
        {
            // Получаем роли пользователя
            var roles = await _userManager.GetRolesAsync(user);

            // Создаем список claims (утверждений) для токена
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),     // ID пользователя
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName), // Имя пользователя
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")      // Номер телефона
            };

            // Добавляем роли как claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)); // Время истечения токена

            // Создаем JWT токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,                                     // Издатель токена
                audience: AuthOptions.AUDIENCE,                                 // Аудитория токена
                claims: claims,                                                 // Данные пользователя
                notBefore: now,                                                 // Недействителен до
                expires: expires,                                               // Время истечения
                signingCredentials: new SigningCredentials(                     // Подпись токена
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            // Преобразуем токен в строку
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Возвращаем токен и время жизни в секундах
            return (encodedJwt, AuthOptions.LIFETIME * 60);
        }
    }
}