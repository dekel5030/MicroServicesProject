using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool IsProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProduction);
            }
        }

        private static void SeedData(AppDbContext? context, bool IsProduction)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "AppDbContext is not registered in the DI container.");
            }

            if (IsProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migration {ex.Message}");
                }
            }
            
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");
                context.Platforms.AddRange(
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Node.js", Publisher = "OpenJS Foundation", Cost = "Free" },
                    new Platform { Name = "Java", Publisher = "Oracle", Cost = "Free" },
                    new Platform { Name = "Python", Publisher = "Python Software Foundation", Cost = "Free" },
                    new Platform { Name = "Ruby on Rails", Publisher = "Rails Core Team", Cost = "Free" },
                    new Platform { Name = "Spring Boot", Publisher = "VMware", Cost = "Free" }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}