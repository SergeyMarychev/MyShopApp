namespace MyShopApp.Application.Contracts.Authorization
{
    public class SmsCodeSettings
    {
        public int CodeLifetimeMinutes { get; set; }
        public int MaxAttempts { get; set; }
        public int CooldownSeconds { get; set; }
    }
}
