using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Opine.Job
{
    public static class Startup
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            Configure(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }

        public static void Configure(IServiceCollection serviceCollection)
        {
            // Load the json config file
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var builder = new ConfigurationBuilder()
                //.SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            ConfigureDependencies(serviceCollection);
        }

        public static void ConfigureDependencies(IServiceCollection serviceCollection)
        {
            serviceCollection
                // Dispatching
                .AddScoped<Dispatching.IHandlerFinder, Dispatching.HandlerFinder>()
                // Handler registration
                .AddSingleton<Dispatching.HandlerRegistry>()
                .AddSingleton<Dispatching.IHandlerRegistry>(s => 
                    s.GetRequiredService<Dispatching.HandlerRegistry>())
                // Message context
                .AddSingleton<Dispatching.MessageContextAccessor>()
                .AddSingleton<Dispatching.IMessageContextAccessor>(s => 
                    s.GetRequiredService<Dispatching.MessageContextAccessor>());
        }
    }
}