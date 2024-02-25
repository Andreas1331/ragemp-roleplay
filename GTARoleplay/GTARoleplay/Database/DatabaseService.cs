using GTARoleplay.Library;
using Microsoft.Extensions.DependencyInjection;

namespace GTARoleplay.Database
{
    public static class DatabaseService
    {
        public static DatabaseBaseContext GetDatabaseContext()
        {
            return ServicesContainer.ServiceProvider.GetRequiredService<DatabaseBaseContext>();
        }
    }
}
