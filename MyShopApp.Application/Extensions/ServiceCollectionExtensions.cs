using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyShopApp.Application.Categories;
using MyShopApp.Application.ProductGroups;
using MyShopApp.Application.Products;
using MyShopApp.Application.Users;

namespace MyShopApp.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient<ICategoryAppService, CategoryAppService>();
            services.AddTransient<IProductAppService, ProductAppService>();
            services.AddTransient<IProductGroupAppService, ProductGroupAppService>();
            services.AddTransient<IUserAppService, UserAppService>();

            return services;
        }
    }
}
