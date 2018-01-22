using System.Threading.Tasks;

namespace Opine.Dispatching
{
    public interface IDispatcher
    {
         Task Dispatch(MessageContext messageContext, object message);
    }
}