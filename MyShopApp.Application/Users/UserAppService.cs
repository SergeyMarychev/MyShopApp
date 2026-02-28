using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Users
{
    internal sealed class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAppService> _logger;

        public UserAppService(IUserRepository userRepository, UserManager<User> userManager, IMapper mapper, ILogger<UserAppService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> GetUserAsync(long userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получение профиля пользователя ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                _logger.LogError("Пользователь с ID {UserId} не найден", userId);
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto input, CancellationToken ct = default)
        {
            _logger.LogInformation("Обновление профиля пользователя ID: {UserId}", input.Id);

            var user = await _userRepository.GetByIdAsync(input.Id, ct);

            if (user == null)
            {
                _logger.LogError("Пользователь с ID {UserId} не найден", input.Id);
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(input.Id);
            }

            // Применяем маппинг из DTO в существующий объект user
            _mapper.Map(input, user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Ошибка обновления пользователя: {Errors}", errors);
                UserFriendlyException.USER_UPDATE_FAILED(errors);
            }

            _logger.LogInformation("Профиль пользователя ID {UserId} успешно обновлен", input.Id);
        }

        public async Task DeleteAccountAsync(long userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Удаление аккаунта пользователя ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                _logger.LogError("Пользователь с ID {UserId} не найден", userId);
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            // Мягкое удаление с записью времени
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow; // ЭТО ВАЖНО!

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Ошибка удаления пользователя: {Errors}", errors);
                UserFriendlyException.USER_DELETION_FAILED(errors);
            }

            _logger.LogInformation("Аккаунт пользователя ID {UserId} успешно удален (soft-delete)", userId);
        }

        // Метод для восстановления аккаунта (может быть вызван из AccountController)
        public async Task<bool> RestoreAccountAsync(string phoneNumber, CancellationToken ct = default)
        {
            _logger.LogInformation("Попытка восстановления аккаунта с номером: {PhoneNumber}", phoneNumber);

            var user = await _userRepository.GetByPhoneNumberIncludeDeletedAsync(phoneNumber, ct);

            if (user == null || !user.IsDeleted)
            {
                return false;
            }

            if (!user.CanBeRestored)
            {
                _logger.LogWarning("Аккаунт с номером {PhoneNumber} не может быть восстановлен (истек срок)", phoneNumber);
                return false;
            }

            user.IsDeleted = false;
            user.DeletedAt = null;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Ошибка восстановления аккаунта: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Аккаунт с номером {PhoneNumber} успешно восстановлен", phoneNumber);
            return true;
        }
    }
}