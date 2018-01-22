using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Dispatching.Static
{
    public class StaticHandlerBase : IStaticHandler
    {
        private Dictionary<Type, Func<MessageContext, object, Task>> handlers = 
            new Dictionary<Type, Func<MessageContext, object, Task>>();

        protected void Register<T>(Func<MessageContext, T, Task> handler)
        {
            if (handlers.ContainsKey(typeof(T)))
                throw new Exception("");

            handlers.Add(typeof(T), (c, m) => handler(c, (T)m));
        }

        public async Task Handle(MessageContext messageContext, object message)
        {
            Func<MessageContext, object, Task> handler = null;
            if (this.handlers.TryGetValue(message.GetType(), out handler))
            {
                await handler(messageContext, message);
                return;
            }
        }
    }
}