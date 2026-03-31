using Microsoft.Extensions.Logging;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Exceptions;
using MyShopApp.Domain.Users;

namespace MyShopApp.Application.Users
{
    internal sealed class ContactService : IContactService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IUserRepository userRepository,
            ILogger<ContactService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ContactResponseDto> ContactAsync(long userId, ContactRequestDto input, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                UserFriendlyException.USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(userId);
            }

            switch (input.Type)
            {
                case ContactType.Chat:
                    return HandleChat(user, input.Message);

                case ContactType.Phone:
                    return HandlePhone(user);

                default:
                    return new ContactResponseDto
                    {
                        Success = false,
                        Message = "Неверный тип обращения",
                        Type = input.Type
                    };
            }
        }

        private ContactResponseDto HandleChat(User user, string? message)
        {
            _logger.LogInformation("==========================================");
            _logger.LogInformation("ЧАТ ПОДДЕРЖКИ");
            _logger.LogInformation("==========================================");
            _logger.LogInformation("Пользователь: {UserName} (ID: {UserId})", user.UserName ?? user.PhoneNumber, user.Id);
            _logger.LogInformation("Телефон: {PhoneNumber}", user.PhoneNumber);
            _logger.LogInformation("Email: {Email}", user.Email ?? "не указан");
            _logger.LogInformation("Сообщение: {Message}", message ?? "пустое сообщение");
            _logger.LogInformation("==========================================");

            return new ContactResponseDto
            {
                Success = true,
                Message = "Ваше сообщение отправлено. Наш специалист свяжется с вами в ближайшее время.",
                Type = ContactType.Chat
            };
        }

        private ContactResponseDto HandlePhone(User user)
        {
            _logger.LogInformation("==========================================");
            _logger.LogInformation("ЗВОНОК В ПОДДЕРЖКУ");
            _logger.LogInformation("==========================================");
            _logger.LogInformation("Пользователь: {UserName} (ID: {UserId})", user.UserName ?? user.PhoneNumber, user.Id);
            _logger.LogInformation("Телефон для связи: {PhoneNumber}", user.PhoneNumber);
            _logger.LogInformation("Email: {Email}", user.Email ?? "не указан");
            _logger.LogInformation("Статус: Инициирован звонок в службу поддержки");
            _logger.LogInformation("==========================================");

            return new ContactResponseDto
            {
                Success = true,
                Message = "Инициирован звонок в службу поддержки. Ожидайте соединения.",
                Type = ContactType.Phone
            };
        }
    }
}
