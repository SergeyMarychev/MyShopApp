using Microsoft.AspNetCore.Identity;
using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Users
{
    public class User : IdentityUser<long>, IEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
    }
}
