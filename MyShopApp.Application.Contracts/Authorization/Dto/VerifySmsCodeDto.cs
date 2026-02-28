namespace MyShopApp.Application.Contracts.Authorization.Dto
{
    public class VerifySmsCodeDto
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
