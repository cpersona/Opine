using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Opine.Dispatching.Static
{
    public class StaticDispatcher : IDispatcher
    {
        private IHandlerRegistry handlerRegistry;
        private IServiceProvider serviceProvider;
        private IUnitOfWork unitOfWork;

        public StaticDispatcher(IHandlerRegistry handlerRegistry, IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            this.handlerRegistry = handlerRegistry;
            this.serviceProvider = serviceProvider;
            this.unitOfWork = unitOfWork;
        }

        public async Task Dispatch(MessageContext messageContext, object message)
        {
            var handlers = handlerRegistry.GetHandlers(message.GetType());
            foreach (var h in handlers)
            {
                var handler = (IStaticHandler)serviceProvider.GetRequiredService(h.MethodInfo.DeclaringType);
                await handler.Handle(messageContext, message);
                await unitOfWork.Commit();
            }
        }
    }
}