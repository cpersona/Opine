using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Opine.Dispatching;
using Opine.Dispatching.Dynamic;
using Opine.Messaging;
using Opine.Messaging.GetEventStore;
using Opine.Plugins;

namespace Opine.Job
{
    class Program
    {
        async static Task Main(string[] args)
        {
            ProgramArgs programArgs = new ProgramArgs();
            try
            {
                // We need to allow unknown options to be accepted, but...
                // the following line fails with an InvalidOperationException
                //CommandLine.Parser.Default.Settings.IgnoreUnknownArguments = true;
                // So we loosely parse command line options
                if (!CommandLine.Parser.Default.ParseArguments(args, programArgs))
                {
                    // Then we check to see if we have the required values (we should)
                    if (string.IsNullOrWhiteSpace(programArgs.StreamName)
                        || string.IsNullOrWhiteSpace(programArgs.StreamType)
                        || !programArgs.AssemblyNames.Any())
                    {
                        // But if we don't, we want to trigger a failure with help text
                        CommandLine.Parser.Default.ParseArgumentsStrict(args, programArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            // Configure job services
            IServiceCollection services = new ServiceCollection();
            Startup.Configure(services);
            // Load assemblies and configure and then save the registry in DI container
            var handlerRegistry = new HandlerRegistry();
            RegisterAssemblies(args, services, programArgs.AssemblyNames, handlerRegistry);
            services.AddSingleton<IHandlerRegistry>(handlerRegistry);
            var serviceProvider = services.BuildServiceProvider();
            // Initialize the stream reader options
            var stream = new Stream(programArgs.StreamName, null, null);
            long position = programArgs?.QueuePosition ?? 0;
            int bufferSize = programArgs?.BufferSize ?? 0;
            var failureStream = new Stream(Categories.Failures);
            // Start reading the messages
            var messageStore = serviceProvider.GetRequiredService<IMessageStore>();
            var messageContextAccessor = serviceProvider.GetRequiredService<MessageContextAccessor>();
            while (true)
            {
                var messages = await messageStore.Read(stream, position, bufferSize);
                foreach (var m in messages)
                {
                    try 
                    {
                        // Create a child scope per message when handling 
                        using (var messageScope = serviceProvider.CreateScope())
                        {
                            // Set the current message context
                            messageContextAccessor.Default = new MessageContext(
                                m.MessageId, m.Metadata.AggregateId, 
                                m.Metadata.ProcessCode, m.Metadata.ProcessId);
                            
                            // Get the dispatcher
                            var dispatcher = messageScope.ServiceProvider.GetRequiredService<IDispatcher>();
                            
                            //if (aggregateErrors.HasError(m.Metadata.AggregateId))
                            if (false)
                            {
                                await messageStore.Store(failureStream, StreamVersion.Any, 
                                    new[] { new StorableMessage(m.Metadata, m.Data) });
                            }
                            else
                            {
                                // Dispatch the message
                                await dispatcher.Dispatch(messageContextAccessor.Default, m.Data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //aggregateErrors.AddError(m.Metadata.AggregateId, ex);
                        await messageStore.Store(failureStream, StreamVersion.Any, 
                            new[] { new StorableMessage(m.Metadata, m.Data) });
                    }
                }
                position += messages.Count();
                if (!messages.Any())
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static void RegisterAssemblies(string[] args, IServiceCollection services, IEnumerable<string> assemblyNames, HandlerRegistry handlerRegistry)
        {
            foreach (var a in assemblyNames)
            {
                var assembly = Assembly.LoadFrom(a);
                var plugins = assembly
                    .DefinedTypes
                    .Where(x => typeof(IPlugin).IsAssignableFrom(x))
                    .ToArray();

                foreach (var p in plugins)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(p);
                    plugin.ParseArguments(args);
                    plugin.RegisterServices(services);
                    plugin.RegisterHandlers(handlerRegistry);
                }
            }
        }
    }
}
