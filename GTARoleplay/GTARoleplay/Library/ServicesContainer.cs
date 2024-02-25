﻿using GTARoleplay.Database;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GTARoleplay.Library
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
            services.AddDbContext<DatabaseBaseContext, InMemoryDatabase>();

            return services;
        }
    }
}
