namespace MyShopApp.Application.Contracts.Users
{
    public class ContactRequestDto
    {
        public ContactType Type { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
