using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unitic_BE.Models
{
    public class UniticDbContextFactory : IDesignTimeDbContextFactory<UniticDbContext>
    {
        public UniticDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Unitic_BE");
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("KhoiConnection");
            var optionsBuilder = new DbContextOptionsBuilder<UniticDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new UniticDbContext(optionsBuilder.Options);
        }
    }
}
