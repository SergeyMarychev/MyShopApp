namespace MyShopApp.Application.Contracts.Email
{
    public class ConfirmEmailResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? RemainingAttempts { get; set; }
        public int? CooldownSeconds { get; set; }
    }
}
