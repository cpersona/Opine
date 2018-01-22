using System;
using System.Linq;
using System.Reflection;

namespace Opine.Dispatching
{
    public class HandlerInfo
    {
        private Lazy<Type[]> parameterTypes = null;

        public HandlerInfo(HandlerType handlerType, Type messageType, string processCode, MethodInfo methodInfo)
        {
            HandlerType = handlerType;
            MessageType = messageType;
            ProcessCode = processCode;
            MethodInfo = methodInfo;

            // Lazy load parameter types
            parameterTypes = new Lazy<Type[]>(() => 
            {
                return MethodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            });
        }

        public HandlerType HandlerType { get; private set; }
        public Type MessageType { get; private set; }
        public string ProcessCode { get; private set; }
        public MethodInfo MethodInfo { get; private set; }

        public Type[] ParameterTypes 
        {
            get 
            {
                return parameterTypes.Value;
            }
        }
    }
}