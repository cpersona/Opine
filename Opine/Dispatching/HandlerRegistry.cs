using System;
using System.Collections.Generic;

namespace Opine.Dispatching
{
    public class HandlerRegistry : IHandlerRegistry
    {
        private Dictionary<Type, List<HandlerInfo>> handlers = 
            new Dictionary<Type, List<HandlerInfo>>();
        
        public void Register(Type type, HandlerInfo handler)
        {
            List<HandlerInfo> current = null;
            if (!this.handlers.TryGetValue(type, out current))
            {
                current = new List<HandlerInfo>();
                this.handlers.Add(type, current);
            }
            current.Add(handler);
        }

        public IEnumerable<HandlerInfo> GetHandlers(Type type)
        {
            List<HandlerInfo> current = null;
            if (this.handlers.TryGetValue(type, out current))
                return current;

            return Array.Empty<HandlerInfo>();
        }
    }
}