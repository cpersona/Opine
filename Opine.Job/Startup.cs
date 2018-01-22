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

            // Load options extensions and bind the ProgramOptions 
            // options to the configuration
            serviceCollection
                .AddOptions()
                .Configure<EventStoreOptions>(configuration.GetSection("EventStore"))
                .Configure<AssemblyOptions>(configuration.GetSection("Assemblies"));

            ConfigureDependencies(serviceCollection);
        }

        public static void ConfigureDependencies(IServiceCollection serviceCollection)
        {
            serviceCollection
                // Dispatching
                //.AddScoped<Dispatching.IDispatcher, Dispatching.Dynamic.DynamicDispatcher>()
                .AddScoped<Dispatching.IHandlerFinder, Dispatching.HandlerFinder>()
                .AddSingleton<Dispatching.HandlerRegistry>()
                .AddSingleton<Dispatching.IHandlerRegistry>(s => 
                    s.GetRequiredService<Dispatching.HandlerRegistry>())
                .AddSingleton<Dispatching.MessageContextAccessor>()
                .AddSingleton<Dispatching.IMessageContextAccessor>(s => 
                    s.GetRequiredService<Dispatching.MessageContextAccessor>())

                /* 
                // EventStore
                .AddScoped<EventStore.ClientAPI.IEventStoreConnection>(s => 
                    {
                        var o = s.GetRequiredService<IOptions<EventStoreOptions>>().Value;
                        var builder = EventStore.ClientAPI.ConnectionSettings.Create()
                            .WithConnectionTimeoutOf(new TimeSpan(1000))
                            .LimitReconnectionsTo(2);

                        //var connection = EventStore.ClientAPI.EventStoreConnection.Create(o.EventStoreUri, builder);
                        var connection = EventStore.ClientAPI.EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
                        connection.ConnectAsync().Wait();
                        return connection;
                    })

                // Messaging
                .AddScoped<Messaging.GetEventStore.ESMessageStore>()
                .AddScoped<Messaging.IMessageStore>(s => 
                    {
                        // We will use a cached message store wrapping an ES store
                        var store = s.GetRequiredService<Messaging.GetEventStore.ESMessageStore>();
                        var cached = new Messaging.CachedMessageStore(store);
                        return cached;
                    })
                */
                ;
        }
    }
}