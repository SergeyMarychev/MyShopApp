using Microsoft.Extensions.Options;
using MyShopApp.Application.Authorization.Settings;

namespace MyShopApp.Application.Authorization
{
    public class AccountRecoveryService
    {
        private readonly AccountSettings _settings;

        public AccountRecoveryService(IOptions<AccountSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Проверяет, можно ли восстановить аккаунт
        /// </summary>
        public bool CanBeRestored(DateTime? deletedAt)
        {
            if (!deletedAt.HasValue)
                return false;

            return deletedAt.Value.AddDays(_settings.AccountRecoveryDays) > DateTime.UtcNow;
        }

        /// <summary>
        /// Возвращает количество оставшихся дней для восстановления
        /// </summary>
        public int GetRemainingDays(DateTime? deletedAt)
        {
            if (!deletedAt.HasValue)
                return 0;

            var expiryDate = deletedAt.Value.AddDays(_settings.AccountRecoveryDays);
            var remaining = (expiryDate - DateTime.UtcNow).Days;

            return Math.Max(0, remaining);
        }

        /// <summary>
        /// Возвращает дату истечения срока восстановления
        /// </summary>
        public DateTime? GetExpiryDate(DateTime? deletedAt)
        {
            return deletedAt?.AddDays(_settings.AccountRecoveryDays);
        }

        /// <summary>
        /// Возвращает количество дней для восстановления из настроек
        /// </summary>
        public int GetRecoveryDays()
        {
            return _settings.AccountRecoveryDays;
        }
    }
}
