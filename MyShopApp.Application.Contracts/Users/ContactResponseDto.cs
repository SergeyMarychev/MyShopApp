namespace MyShopApp.Application.Contracts.Users
{
    public class ContactResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ContactType Type { get; set; }
    }
}
