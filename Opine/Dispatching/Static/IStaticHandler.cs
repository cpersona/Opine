using System.Threading.Tasks;

namespace Opine.Dispatching.Static
{
    // Static dispatcher will use this to call
    public interface IStaticHandler
    {
        Task Handle(MessageContext messageContext, object message);
    }
}