using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Opine.Domain;
using Opine.Messaging;
using Opine.Repositories;
using Opine.Snapshots;

namespace Opine.Dispatching.Dynamic
{
    public class DynamicDispatcher : IDispatcher
    {
        private IHandlerRegistry handlerRegistry;
        private IServiceProvider serviceProvider;
        private IUnitOfWork unitOfWork;

        public DynamicDispatcher(IHandlerRegistry handlerRegistry, IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            this.handlerRegistry = handlerRegistry;
            this.serviceProvider = serviceProvider;
            this.unitOfWork = unitOfWork;
        }

        public async Task Dispatch(MessageContext messageContext, object message)
        {
            var type = message.GetType();
            var handlers = handlerRegistry.GetHandlers(type);
            await Handle(messageContext, message, handlers);
        }

        private async Task Handle(MessageContext messageContext, object message, IEnumerable<HandlerInfo> handlers)
        {
            foreach (var h in handlers)
            {
                var parameters = await GetParameters(messageContext, message, h);
                await Handle(h, parameters);
                await Save(messageContext, h, parameters);
                await unitOfWork.Commit();
            }
        }

        private async Task Handle(HandlerInfo handler, object[] parameters)
        {
            var mi = handler.MethodInfo;
            var handlerInstance = serviceProvider.GetRequiredService(mi.DeclaringType);
            var result = mi.Invoke(handlerInstance, parameters);
            if (result is Task t)
            {
                await t;
            }
        }

        private async Task<object[]> GetParameters(MessageContext messageContext, object message, HandlerInfo handler)
        {
            var result = new List<object>();
            foreach (var t in handler.ParameterTypes)
            {
                if (typeof(IAggregate).IsAssignableFrom(t))
                {
                    var loader = serviceProvider.GetRequiredService<IRepository>();
                    var aggregate = await loader.Load(t, messageContext.AggregateId);
                    result.Add(aggregate);
                }
                else if (t == typeof(MessageContext))
                {
                    result.Add(messageContext);
                }
                else if (t == message.GetType())
                {
                    result.Add(message);
                }
                else
                {
                    var parameter = serviceProvider.GetRequiredService(t);
                    result.Add(parameter);
                }
            }
            return result.ToArray();
        }

        private async Task Save(MessageContext messageContext, HandlerInfo handler, object[] parameters)
        {
            foreach (var p in parameters)
            {
                switch (p)
                {
                    case IAggregate a: 
                        var repository = serviceProvider.GetRequiredService<IRepository>();
                        await repository.Save(messageContext, a);
                        break;
                    default: 
                        break;
                }
            }
        }
    }
}