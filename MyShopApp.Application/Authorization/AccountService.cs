using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyShopApp.Application.Authorization.Models;
using MyShopApp.Application.Authorization.Settings;
using MyShopApp.Application.Cache;
using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace MyShopApp.Application.Authorization
{
    internal sealed class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository; 
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountService> _logger;
        private readonly SmsCodeSettings _smsSettings;
        private readonly IDistributedCache _cache;
        private readonly AccountRecoveryService _recoveryService;

        public AccountService(
            IUserRepository userRepository, 
            UserManager<User> userManager,
            ILogger<AccountService> logger,
            IOptions<SmsCodeSettings> smsSettings,
            IDistributedCache cache,
            AccountRecoveryService recoveryService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
            _smsSettings = smsSettings.Value;
            _cache = cache;
            _recoveryService = recoveryService;
        }

        private async Task SetCodeDataAsync(string phoneNumber, SmsCodeData data, CancellationToken ct = default)
        {
            var cacheKey = CacheHelper.GetSmsCodeKey(phoneNumber);
            var serialized = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_smsSettings.CodeLifetimeMinutes)
            };
            await _cache.SetStringAsync(cacheKey, serialized, options, ct);
        }

        private async Task<SmsCodeData?> GetCodeDataAsync(string phoneNumber, CancellationToken ct = default)
        {
            var cacheKey = CacheHelper.GetSmsCodeKey(phoneNumber);
            var cachedData = await _cache.GetStringAsync(cacheKey, ct);
            return string.IsNullOrEmpty(cachedData) ? null : JsonSerializer.Deserialize<SmsCodeData>(cachedData);
        }

        private async Task RemoveCodeDataAsync(string phoneNumber, CancellationToken ct = default)
        {
            var cacheKey = CacheHelper.GetSmsCodeKey(phoneNumber);
            await _cache.RemoveAsync(cacheKey, ct);
        }

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task<TokenDto> GenerateJwtTokenAsync(User user)
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

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokenDto
            {
                Token = encodedJwt,
                ExpiresIn = AuthOptions.LIFETIME * 60
            };
        }

        private async Task<User> GetOrCreateUserAsync(string phoneNumber, CancellationToken ct)
        {
            // Используем репозиторий для поиска пользователя (включая удаленных)
            var user = await _userRepository.GetByPhoneNumberIncludeDeletedAsync(phoneNumber, ct);

            if (user == null)
            {
                // Создаём нового пользователя
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    UserName = phoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user, ct);
                await _userRepository.UnitOfWork.SaveChangesAsync(ct);

                _logger.LogInformation("Создан новый пользователь с ID: {UserId}", user.Id);
            }
            else if (user.IsDeleted)
            {
                // Проверяем, можно ли восстановить аккаунт
                if (_recoveryService.CanBeRestored(user.DeletedAt))
                {
                    // Восстанавливаем удалённого пользователя
                    user.IsDeleted = false;
                    user.DeletedAt = null;
                    _userRepository.Update(user);
                    await _userRepository.UnitOfWork.SaveChangesAsync(ct);

                    _logger.LogInformation("Аккаунт восстановлен для номера: {PhoneNumber}", phoneNumber);
                }
                else
                {
                    // Если срок восстановления истек - создаем нового пользователя
                    _logger.LogInformation("Срок восстановления истек. Создаем новый аккаунт для номера: {PhoneNumber}", phoneNumber);

                    var newUser = new User
                    {
                        PhoneNumber = phoneNumber,
                        UserName = phoneNumber,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(newUser, ct);
                    await _userRepository.UnitOfWork.SaveChangesAsync(ct);

                    user = newUser;
                    _logger.LogInformation("Создан новый пользователь (старый удален >30 дней) с ID: {UserId}", user.Id);
                }
            }

            return user;
        }

        public async Task<LoginResultDto> RequestCodeAsync(string phoneNumber, CancellationToken ct = default)
        {
            _logger.LogInformation("Запрос кода для номера: {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                UserFriendlyException.PHONE_NUMBER_CAN_NOT_BE_EMPTY();
            }

            // Проверяем кулдаун
            var existingData = await GetCodeDataAsync(phoneNumber, ct);
            if (existingData != null)
            {
                if (existingData.CooldownUntil.HasValue && existingData.CooldownUntil > DateTime.UtcNow)
                {
                    var secondsLeft = (int)(existingData.CooldownUntil.Value - DateTime.UtcNow).TotalSeconds;
                    _logger.LogWarning("Кулдаун для номера {PhoneNumber}: {SecondsLeft} секунд", phoneNumber, secondsLeft);

                    return new LoginResultDto
                    {
                        PhoneNumber = phoneNumber,
                        CooldownSeconds = secondsLeft,
                        RequiresCooldown = true
                    };
                }

                if (existingData.Expiry < DateTime.UtcNow || existingData.Attempts >= _smsSettings.MaxAttempts)
                {
                    await RemoveCodeDataAsync(phoneNumber, ct);
                }
            }

            var code = GenerateCode();

            var newData = new SmsCodeData
            {
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(_smsSettings.CodeLifetimeMinutes),
                Attempts = 0,
                CooldownUntil = DateTime.UtcNow.AddSeconds(_smsSettings.CooldownSeconds),
                PhoneNumber = phoneNumber
            };

            await SetCodeDataAsync(phoneNumber, newData, ct);

            _logger.LogInformation("Для номера {PhoneNumber} сгенерирован код: {Code}", phoneNumber, code);

            return new LoginResultDto
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<TokenDto> VerifyCodeAsync(VerifySmsCodeDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Проверка кода для номера: {PhoneNumber}", input.PhoneNumber);

            // 1. Проверяем код
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

            await RemoveCodeDataAsync(input.PhoneNumber, ct);

            // 2. Получаем или создаем пользователя через репозиторий
            var user = await GetOrCreateUserAsync(input.PhoneNumber, ct);

            // 3. Генерируем JWT токен
            return await GenerateJwtTokenAsync(user);
        }
    }
}