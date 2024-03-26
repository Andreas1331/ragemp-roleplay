using GTANetworkAPI;
using GTARoleplay.Database;
using GTARoleplay.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GTARoleplay.Library
{
    public class Main : Script
    {
        public Main()
        {
            var provider = new ServiceContainer();
            provider.InitAllSingletons();

            using (var dbCtx = provider.ServiceProvider.GetRequiredService<DatabaseBaseContext>())
            {
                // Migrate the database so it's up to date, or ensure it's created if using InMemory.
                if (dbCtx.Database.IsRelational())
                    dbCtx.Database.Migrate();
                else
                    dbCtx.Database.EnsureCreated();
            }
        }
    }
}
