using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MekanikApi.Domain.Entities;
using MekanikApi.Domain.GenericRepository;
using MekanikApi.Domain.Interfaces;
using MekanikApi.Infrastructure.DataContext;
using MekanikApi.Infrastructure.HttpClient;
using MekanikApi.Infrastructure.Services;
using MekanikApi.Infrastructure.Repository;

namespace MekanikApi.Infrastructure
{
    public static class ConfigureInfrastructureService
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Environment.GetEnvironmentVariable("postgressdb"),
                    b => b.MigrationsAssembly("MekanikApi.Api").UseNetTopologySuite());
                options.EnableDetailedErrors(true);
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            
            

            // Register other services and repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddMemoryCache();
            //services.AddHostedService<StartupApicallService>();

            return services;
        }
    }
}