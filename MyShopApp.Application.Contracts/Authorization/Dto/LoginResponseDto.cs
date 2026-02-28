namespace MyShopApp.Application.Contracts.Authorization.Dto
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public long UserId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsNewUser { get; set; }
        public bool IsDeletedUser { get; set; }
        public bool IsRestored { get; set; }
    }
}
