namespace MyShopApp.Application.Contracts.Authorization.Dto
{
    public class LoginResultDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public int? CooldownSeconds { get; set; }
        public bool RequiresCooldown { get; set; }
    }
}
