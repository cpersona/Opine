namespace Opine.Dispatching
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class HandlerMethodAttribute : System.Attribute
    {
        public bool IsProcessStart { get; set; }
    }
}