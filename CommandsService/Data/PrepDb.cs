using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serverScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serverScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient!.ReturnAllPlatforms();

                SeedData(serverScope.ServiceProvider.GetService<ICommandRepo>()!, platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform>? platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");

            if (platforms == null)
            {
                return;
            }

            foreach (var plat in platforms)
            {
                if (!repo.PlatformExists(plat.Id))
                {
                    repo.CreatePlatform(plat);
                }
            }

            repo.SaveChanges();
        }
    }
}