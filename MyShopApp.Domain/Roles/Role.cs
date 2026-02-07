using Microsoft.AspNetCore.Identity;
using MyShopApp.Domain.Common;

namespace MyShopApp.Domain.Roles
{
    public class Role : IdentityRole<long>, IEntity
    {
        public ICollection<RoleClaim> Claims { get; set; }
    }
}
