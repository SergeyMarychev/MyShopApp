using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.Users
{
    public class UpdateUserDto : EntityDto
    {
        public string Name { get; set; }
        public bool AllowSharingData { get; set; }
        public bool AllowPushNotifications { get; set; }
        public bool AllowPushEmails { get; set; }
        public bool AllowPushSms { get; set; }
    }
}
