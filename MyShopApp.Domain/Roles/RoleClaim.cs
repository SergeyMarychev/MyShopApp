using Microsoft.AspNetCore.Identity;

namespace MyShopApp.Domain.Roles
{
    public class RoleClaim : IdentityRoleClaim<long>
    {
        public Role Role { get; set; }
    }
}
