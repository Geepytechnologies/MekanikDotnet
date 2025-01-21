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
    public static class VehicleBrandSeeder
    {
        public static void Seed(IServiceProvider provider)
        {
            // Path to the JSON file
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "carBrands.json");

            using var context = new ApplicationDbContext(
                provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var jsonData = File.ReadAllText(jsonFilePath);
            var brands = JsonConvert.DeserializeObject<List<VehicleBrand>>(jsonData);
            if (context.VehicleBrands.Any())
            {
                return;
            }
            if (brands != null)
            {
                var vehicleBrands = brands.Select(b => new VehicleBrand
                {
                    Name = b.Name
                }).ToList();

                context.AddRange(vehicleBrands);

                context.SaveChanges();
            }
        }
    }
}
