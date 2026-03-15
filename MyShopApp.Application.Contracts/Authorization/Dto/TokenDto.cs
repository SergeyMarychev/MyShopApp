namespace MyShopApp.Application.Contracts.Authorization.Dto
{
    public class TokenDto
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
