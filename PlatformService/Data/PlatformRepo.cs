using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _Context;

        public PlatformRepo(AppDbContext context)
        {
            _Context = context;
        }
        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _Context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _Context.Platforms.ToList();
        }

        public Platform? GetPlatformById(int id)
        {
            var platform = _Context.Platforms.FirstOrDefault(p => p.Id == id);

            return platform;
        }

        public bool SaveChanges()
        {
            return _Context.SaveChanges() >= 0 ? true : false;
        }
    }
}