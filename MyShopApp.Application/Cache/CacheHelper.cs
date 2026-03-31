namespace MyShopApp.Application.Cache
{
    public static class CacheHelper
    {
        private const string SMS_CODE_KEY_PREFIX = "sms_code_";
        private const string EMAIL_CONFIRMATION_KEY_PREFIX = "email_confirmation_";

        /// <summary>
        /// Формирует ключ для хранения SMS кода в кэше
        /// </summary>
        /// <returns>Ключ для кэша</returns>
        public static string GetSmsCodeKey(string phoneNumber)
        {
            return $"{SMS_CODE_KEY_PREFIX}{phoneNumber}";
        }

        /// <summary>
        /// Формирует ключ для хранения кода подтверждения email в кэше
        /// </summary>
        /// <returns>Ключ для кэша</returns>
        public static string GetEmailConfirmationKey(long userId, string email)
        {
            return $"{EMAIL_CONFIRMATION_KEY_PREFIX}{userId}_{email}";
        }
    }
}
