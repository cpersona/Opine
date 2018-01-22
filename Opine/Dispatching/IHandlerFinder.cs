using System;
using System.Collections.Generic;

namespace Opine.Dispatching
{
    public interface IHandlerFinder
    {
         IEnumerable<HandlerInfo> FindHandlers(Type type);
    }
}