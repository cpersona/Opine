namespace Opine.Dispatching
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class HandlerClassAttribute : System.Attribute
    {
        public HandlerClassAttribute(HandlerType handlerType)
        {
            HandlerType = handlerType;
        }

        public HandlerType HandlerType { get; private set; }
        public string ProcessCode { get; set; }
    }
}