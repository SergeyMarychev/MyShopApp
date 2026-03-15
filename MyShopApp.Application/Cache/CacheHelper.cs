namespace MyShopApp.Application.Cache
{
    public static class CacheHelper
    {
        private const string SMS_CODE_KEY_PREFIX = "sms_code_";

        /// <summary>
        /// Формирует ключ для хранения SMS кода в кэше
        /// </summary>
        /// <returns>Ключ для кэша</returns>
        public static string GetSmsCodeKey(string phoneNumber)
        {
            return $"{SMS_CODE_KEY_PREFIX}{phoneNumber}";
        }
    }
}
