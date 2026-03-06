namespace MyShopApp.Application.Authorization.Settings
{
    public class SmsCodeSettings
    {
        public int CodeLifetimeMinutes { get; set; }
        public int MaxAttempts { get; set; }
        public int CooldownSeconds { get; set; }
    }
}
