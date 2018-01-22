using System;
using System.Collections.Generic;

namespace Opine.Dispatching
{
    public interface IHandlerRegistry
    {
        void Register(Type type, HandlerInfo handler);
        IEnumerable<HandlerInfo> GetHandlers(Type type);
    }
}