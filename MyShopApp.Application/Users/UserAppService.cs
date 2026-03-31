using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyShopApp.Application.Cache;
using MyShopApp.Application.Contracts.Email;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Users;
using System.Text.Json;

namespace MyShopApp.Application.Users
{
    internal sealed class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAppService> _logger;
        private readonly IDistributedCache _cache;
        private readonly EmailCodeSettings _emailSettings;

        public UserAppService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IMapper mapper,
            ILogger<UserAppService> logger,
            IDistributedCache cache,
            IOptions<EmailCodeSettings> emailSettings)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _emailSettings = emailSettings.Value;
        }

        public async Task<UserDto> GetAsync(long userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получение профиля пользователя ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(UpdateUserDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Обновление профиля пользователя ID: {UserId}", input.Id);

            var user = await _userRepository.GetByIdAsync(input.Id, ct);

            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            // Сохраняем старый email для проверки
            var oldEmail = user.Email;
            var newEmail = input.Email;

            // Обновляем только имя и настройки (email не обновляем в БД!)
            user.UserName = input.Name;
            user.AllowSharingData = input.AllowSharingData;
            user.AllowPushNotifications = input.AllowPushNotifications;
            user.AllowPushEmails = input.AllowPushEmails;
            user.AllowPushSms = input.AllowPushSms;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                UserFriendlyException.USER_UPDATE_FAILED(errors);
            }

            // Если email изменился, отправляем код подтверждения
            if (!string.IsNullOrEmpty(newEmail) && oldEmail != newEmail)
            {
                // Проверяем, не занят ли email другим пользователем
                var existingUser = await _userManager.FindByEmailAsync(newEmail);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    UserFriendlyException.EMAIL_ALREADY_IN_USE();
                }

                // Отправляем код подтверждения
                await SendEmailConfirmationCodeAsync(user.Id, newEmail, ct);

                _logger.LogInformation("Для пользователя {UserId} запрошено подтверждение email {NewEmail}", user.Id, newEmail);
            }

            _logger.LogInformation("Профиль пользователя ID {UserId} успешно обновлен", input.Id);
        }

        public async Task DeleteAsync(long userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Удаление аккаунта пользователя ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                UserFriendlyException.USER_DELETION_FAILED(errors);
            }

            _logger.LogInformation("Аккаунт пользователя ID {UserId} успешно удален", userId);
        }

        public async Task<ConfirmEmailResultDto> ConfirmEmailAsync(long userId, ConfirmEmailDto input, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            var cacheKey = CacheHelper.GetEmailConfirmationKey(userId, input.Email);
            var cachedData = await _cache.GetStringAsync(cacheKey, ct);


            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogWarning("!!! КОД В КЭШЕ НЕ НАЙДЕН !!!");
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    Message = "Код подтверждения не найден или истек. Обновите профиль снова."
                };
            }

            var codeData = JsonSerializer.Deserialize<EmailCodeData>(cachedData);

            // Проверяем кулдаун
            if (codeData.CooldownUntil.HasValue && codeData.CooldownUntil > DateTime.UtcNow)
            {
                var secondsLeft = (int)(codeData.CooldownUntil.Value - DateTime.UtcNow).TotalSeconds;
                _logger.LogWarning("Попытка в период кулдауна. Осталось {SecondsLeft} секунд", secondsLeft);
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    Message = $"Слишком много попыток. Подождите {secondsLeft} секунд.",
                    CooldownSeconds = secondsLeft
                };
            }

            if (codeData.Expiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Код истек");
                await _cache.RemoveAsync(cacheKey, ct);
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    Message = "Код подтверждения истек. Обновите профиль снова."
                };
            }

            // Увеличиваем счетчик попыток
            codeData.Attempts++;

            if (codeData.Code != input.Code)
            {
                var remainingAttempts = _emailSettings.MaxAttempts - codeData.Attempts;
                _logger.LogWarning("!!! НЕВЕРНЫЙ КОД !!! Осталось попыток: {RemainingAttempts}", remainingAttempts);

                var serialized = JsonSerializer.Serialize(codeData);
                var codeLifetimeMinutes = _emailSettings.CodeLifetimeMinutes > 0 ? _emailSettings.CodeLifetimeMinutes : 5;
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(codeLifetimeMinutes)
                };
                await _cache.SetStringAsync(cacheKey, serialized, cacheOptions, ct);

                return new ConfirmEmailResultDto
                {
                    Success = false,
                    Message = $"Неверный код. Осталось попыток: {remainingAttempts}",
                    RemainingAttempts = remainingAttempts
                };
            }

            // Код верный, обновляем email пользователя
            _logger.LogInformation("!!! КОД ВЕРНЫЙ !!! Обновляем email пользователя");

            var oldEmail = user.Email;
            user.Email = input.Email;
            user.EmailConfirmed = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Ошибка обновления email: {Errors}", errors);
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    Message = "Ошибка при обновлении email"
                };
            }

            await _cache.RemoveAsync(cacheKey, ct);

            _logger.LogInformation("Email пользователя {UserId} успешно изменен с {OldEmail} на {NewEmail}", userId, oldEmail, input.Email);

            return new ConfirmEmailResultDto
            {
                Success = true,
                Message = "Email успешно подтвержден и обновлен"
            };
        }

        private async Task SendEmailConfirmationCodeAsync(long userId, string email, CancellationToken ct)
        {
            var cacheKey = CacheHelper.GetEmailConfirmationKey(userId, email);

            // Проверяем существующий код
            var existingData = await _cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(existingData))
            {
                var codeData = JsonSerializer.Deserialize<EmailCodeData>(existingData);

                if (codeData.CooldownUntil.HasValue && codeData.CooldownUntil > DateTime.UtcNow)
                {
                    var secondsLeft = (int)(codeData.CooldownUntil.Value - DateTime.UtcNow).TotalSeconds;
                    UserFriendlyException.EMAIL_CONFIRMATION_COOLDOWN(secondsLeft);
                }

                if (codeData.Expiry < DateTime.UtcNow || codeData.Attempts >= _emailSettings.MaxAttempts)
                {
                    await _cache.RemoveAsync(cacheKey, ct);
                }
                else
                {
                    UserFriendlyException.EMAIL_CODE_ALREADY_SENT();
                }
            }

            // Используем значение из настроек или значение по умолчанию
            var codeLifetimeMinutes = _emailSettings.CodeLifetimeMinutes > 0 ? _emailSettings.CodeLifetimeMinutes : 5;

            // Генерируем новый код
            var code = GenerateCode();
            var expiryUtc = DateTime.UtcNow.AddMinutes(codeLifetimeMinutes);
            var expiryLocal = expiryUtc.ToLocalTime();

            var newCodeData = new EmailCodeData
            {
                Code = code,
                Expiry = expiryUtc,
                Attempts = 0,
                Email = email,
                UserId = userId,
                CooldownUntil = null
            };

            var serialized = JsonSerializer.Serialize(newCodeData);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(codeLifetimeMinutes)
            };

            await _cache.SetStringAsync(cacheKey, serialized, cacheOptions, ct);

            _logger.LogInformation("На {Email} отправлен код: {Code}", email, code);
        }

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}