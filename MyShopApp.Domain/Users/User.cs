using Microsoft.AspNetCore.Identity;
using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public class User : IdentityUser<long>, IEntity
    {
        public bool AllowSharingData { get; set; }
        public bool AllowPushNotifications { get; set; }
        public bool AllowPushEmails { get; set; }
        public bool AllowPushSms { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; } 

        // Свойство для проверки возможности восстановления (30 дней)
        public bool CanBeRestored => DeletedAt.HasValue && DeletedAt.Value.AddDays(30) > DateTime.UtcNow;
    }
}
