using GTARoleplay.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace GTARoleplay.Database
{
    public static class DatabaseService
    {
        public static DatabaseBaseContext GetDatabaseContext()
        {
            return ServicesContainer.ServiceProvider.GetRequiredService<DatabaseBaseContext>();
        }

        public static void EnsureDatabaseIsCreated()
        {
            var ctx = ServicesContainer.ServiceProvider.GetRequiredService<DatabaseBaseContext>();
            if (!ctx.IsRelational())
                ctx.Database.EnsureCreated();
        }
    }
}
