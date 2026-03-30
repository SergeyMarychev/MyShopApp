using MyShopApp.Application.Contracts.Common.Dto;

namespace MyShopApp.Application.Contracts.Users
{
    public class AddressDto : EntityDto
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public int ApartmentNumber { get; set; }
        public int OfficeNumber { get; set; }
        public int FloorNumber { get; set; }
        public int HouseSectionNumber { get; set; }
        public int DoorphoneNumber { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
