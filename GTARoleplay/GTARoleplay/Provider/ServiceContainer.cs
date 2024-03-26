using GTARoleplay.Account;
using GTARoleplay.AdminSystem;
using GTARoleplay.Character;
using GTARoleplay.Character.Customization;
using GTARoleplay.Database;
using GTARoleplay.Database.Providers;
using GTARoleplay.Events;
using GTARoleplay.FactionSystem;
using GTARoleplay.Interactions;
using GTARoleplay.InventorySystem;
using GTARoleplay.Library;
using GTARoleplay.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Provider
{
    public class ServiceContainer
    {
        public readonly ServiceProvider ServiceProvider;

        private readonly IServiceCollection serviceCollection;

        public ServiceContainer ()
        {
            serviceCollection = new ServiceCollection()
                .AddDbContext<DatabaseBaseContext, InMemoryDatabase>(
                contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton)
                .AddSingleton<EventsHandler>()
                .AddSingleton<AccountHandler>()
                .AddSingleton<AdminHandler>()
                .AddSingleton<CharacterHandler>()
                .AddSingleton<ItemHandler>()
                .AddSingleton<VehicleHandler>()
                .AddSingleton<VehicleModEventHandler>()
                .AddSingleton<PlayerHandler>()
                .AddSingleton<CustomizationHandler>()
                .AddSingleton<FactionHandler>()
                .AddSingleton<AdminInventoryCommands>()
                .AddSingleton<AdminPlayerCommands>()
                .AddSingleton<InteractionHandler>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public void InitAllSingletons()
        {
            var singletonTypes = GetAllSingletonTypes();
            foreach (var type in singletonTypes)
                ServiceProvider.GetRequiredService(type);
        }

        public IEnumerable<Type> GetAllSingletonTypes()
            => serviceCollection.Where(s => s.Lifetime == ServiceLifetime.Singleton).Select(s => s.ServiceType);

    }
}
