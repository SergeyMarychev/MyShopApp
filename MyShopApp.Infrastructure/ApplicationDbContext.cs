using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShopApp.Domain.Categories;
using MyShopApp.Domain.ProductGroups;
using MyShopApp.Domain.Products;
using MyShopApp.Domain.Roles;
using MyShopApp.Domain.Users;

namespace MyShopApp.Infrastructure
{
    public class ApplicationDbContext : 
        IdentityDbContext<User, Role, long, 
        IdentityUserClaim<long>, IdentityUserRole<long>, 
        IdentityUserLogin<long>, RoleClaim, IdentityUserToken<long>>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductGroupProduct> ProductGroupProducts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }
}
