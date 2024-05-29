using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace PROG7311POE.Data
{
    /// <summary>
    /// EF Core commands are broken :\
    /// Basically during design time, when a database is created using the `dotnet ef migrations add` command, 
    /// dotnet cannot understand the dbcontext when using DI and needs help to work with the DI container.
    /// </summary>
    public class DataContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var myOptionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var myConnectionString = configuration.GetConnectionString("MyDefaultConnection");

            myOptionsBuilder.UseSqlite(myConnectionString);

            return new MyDbContext(myOptionsBuilder.Options);
        }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻