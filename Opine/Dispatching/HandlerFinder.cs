using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Opine.Domain;

namespace Opine.Dispatching
{
    public class HandlerFinder : IHandlerFinder
    {
        public IEnumerable<HandlerInfo> FindHandlers(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                foreach (var hi in FindHandlers(type))
                {
                    yield return hi;
                }
            }
        }

        public IEnumerable<HandlerInfo> FindHandlers(Type type)
        {
            var handlerClassAttribute = type
                .GetCustomAttributes(typeof(HandlerClassAttribute), true)
                .Cast<HandlerClassAttribute>()
                .SingleOrDefault();
            if (handlerClassAttribute == null) return Enumerable.Empty<HandlerInfo>();

            var handlers = type
                // Get all methods
                .GetMethods()
                // That have a handler attribute
                .Select(x => 
                    new 
                    { 
                        MethodInfo = x,  
                        HandlerMethodAttribute = x
                            .GetCustomAttributes(typeof(HandlerMethodAttribute), true)
                            .Cast<HandlerMethodAttribute>()
                            .SingleOrDefault()
                    })
                .Where(x => x.HandlerMethodAttribute != null)
                // Get the method and the type it handles
                .Select(x => 
                    new 
                    {
                        x.MethodInfo,
                        MessageType = 
                            x.MethodInfo
                                .GetParameters()
                                .Where(y => typeof(IEvent).IsAssignableFrom(y.ParameterType)
                                    || typeof(ICommand).IsAssignableFrom(y.ParameterType))
                                .Select(y => y.ParameterType)
                                .Single(),
                        IsProcessStart = x?.HandlerMethodAttribute?.IsProcessStart ?? false,
                    });

            return handlers
                .Select(x => 
                    new HandlerInfo(
                        handlerClassAttribute.HandlerType, 
                        x.MessageType, 
                        (x.IsProcessStart) ? string.Empty : handlerClassAttribute.ProcessCode, 
                        x.MethodInfo));
        }
    }
}