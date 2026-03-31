namespace MyShopApp.Application.Contracts.Email
{
    public class EmailCodeSettings
    {
        public int CodeLifetimeMinutes { get; set; }
        public int MaxAttempts { get; set; }
        public int CooldownSeconds { get; set; }
    }
}
