using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppGastos.Api.Data;

// Usada solo por las herramientas de diseño de EF Core (dotnet ef migrations add/update)
// para no tener que ejecutar Program.cs -y su Database.Migrate() contra una base real- en ese momento.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Default"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
