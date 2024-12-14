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
                    b => b.MigrationsAssembly("MekanikApi.Api"));
                options.EnableDetailedErrors(true);
            });

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddHttpClient("termii", (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.BaseAddress = new Uri("https://v3.api.termii.com");
            });
            services.AddHttpClient("tokenClient", httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.BaseAddress = new Uri("https://api.sandbox.safehavenmfb.com/");
            });
            services.AddHttpClient("vtu", httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.BaseAddress = new Uri("https://subandgain.com/api/");
            });
            services.AddHttpClient("safehavencontactless", httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.BaseAddress = new Uri("https://api.safehavenmc.com/pos/contactless");
            });
            

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