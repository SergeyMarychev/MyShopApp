using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.Users
{
    public class CurrentUserInfoDto : EntityDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public AddressDto LastAddress { get; set; }
    }
}
