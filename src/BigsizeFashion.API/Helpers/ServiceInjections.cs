using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Business.Services;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using BigSizeFashion.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Helpers
{
    public static class ServiceInjections
    {
        /// <summary>
        /// Inject services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServiceInjection(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Config for Database
            services.AddDbContext<BigSizeFashionChainContext>
            (
                options => options.UseSqlServer(configuration.GetConnectionString("BigSizeFashionConnectionString"))
            );

            // Config for Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Config for Services Dependency Injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<IColourService, ColourService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IPromotionDetailService, PromotionDetailService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductDetailService, ProductDetailService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            return services;
        }
    }
}
