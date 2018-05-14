using Opine.Messaging;

namespace Opine.Dispatching
{
    public interface IMessageContextFactory
    {
         MessageContext Create(StoredMessage message);
    }
}