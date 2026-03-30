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
        private readonly IAddressRepository _addressRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAppService> _logger;

        public UserAppService(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            UserManager<User> userManager, 
            IMapper mapper, 
            ILogger<UserAppService> logger)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> GetAsync(long userId, CancellationToken ct = default)
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

        public async Task UpdateAsync(UpdateUserDto input, CancellationToken ct = default)
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

        public async Task DeleteAsync(long userId, CancellationToken ct = default)
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
            user.DeletedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Ошибка удаления пользователя: {Errors}", errors);
                UserFriendlyException.USER_DELETION_FAILED(errors);
            }

            _logger.LogInformation("Аккаунт пользователя ID {UserId} успешно удален (soft-delete)", userId);
        }

        public async Task<CurrentUserInfoDto> GetCurrentUserInfoAsync(long userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Получение информации о текущем пользователе ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                _logger.LogError("Пользователь с ID {UserId} не найден", userId);
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            var result = new CurrentUserInfoDto
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                LastAddress = await GetLastAddressAsync(userId, ct)
            };

            return result;
        }

        private async Task<AddressDto> GetLastAddressAsync(long userId, CancellationToken ct)
        {
            var lastAddress = await _addressRepository.GetLastAddressByUserIdAsync(userId, ct);

            if (lastAddress == null)
            {
                return null;
            }                

            return new AddressDto
            {
                Id = lastAddress.Id,
                City = lastAddress.City,
                Street = lastAddress.Street,
                HouseNumber = lastAddress.HouseNumber,
                ApartmentNumber = lastAddress.ApartmentNumber,
                OfficeNumber = lastAddress.OfficeNumber,
                FloorNumber = lastAddress.FloorNumber,
                HouseSectionNumber = lastAddress.HouseSectionNumber,
                DoorphoneNumber = lastAddress.DoorphoneNumber,
                Comment = lastAddress.Comment,
                CreatedAt = lastAddress.CreatedAt
            };
        }
    }
}