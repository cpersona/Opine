using System;
using System.Collections.Generic;
using System.Reflection;

namespace Opine.Dispatching
{
    public interface IHandlerFinder
    {
        IEnumerable<HandlerInfo> FindHandlers(Assembly assembly);
        IEnumerable<HandlerInfo> FindHandlers(Type type);
    }
}