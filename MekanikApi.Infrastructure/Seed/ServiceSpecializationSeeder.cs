using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Seed
{
    public static class ServiceSpecializationSeeder
    {
        public static void Seed(IServiceProvider provider)
        {
            // Path to the JSON file
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "services.json");

            using var context = new ApplicationDbContext(
                provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var jsonData = File.ReadAllText(jsonFilePath);
            var services = JsonConvert.DeserializeObject<List<Service>>(jsonData);
            if (context.Services.Any())
            {
                return;
            }
            if (services != null)
            {
                var appServices = services.Select(b => new Service
                {
                    Name = b.Name,
                    Description = b.Description
                }).ToList();

                context.AddRange(appServices);

                context.SaveChanges();
            }
        }
    }
}
