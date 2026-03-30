using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public sealed class UserAddress : Entity, IHasCreationTime
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long AddressId { get; set; }
        public Address Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
