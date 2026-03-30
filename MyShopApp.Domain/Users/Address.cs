using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public sealed class Address : Entity, IHasCreationTime
    {
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public int HouseNumber { get; set; }
        public int ApartmentNumber { get; set; }
        public int OfficeNumber { get; set; }
        public int FloorNumber { get; set; }
        public int HouseSectionNumber { get; set; }
        public int DoorphoneNumber { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    }
}
