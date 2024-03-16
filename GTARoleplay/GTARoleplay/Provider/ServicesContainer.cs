using GTARoleplay.Database;
using GTARoleplay.Database.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GTARoleplay.Provider
{
    public static class ServicesContainer
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static IServiceProvider Init()
        {
            var serviceProvider = new ServiceCollection()
                .ConfigureServices()
                .BuildServiceProvider();

            ServiceProvider = serviceProvider;

            return serviceProvider;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddDbContext<DatabaseBaseContext, InMemoryDatabase>(
                contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

            return services;
        }
    }
}
