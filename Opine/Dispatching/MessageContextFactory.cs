using Opine.Messaging;

namespace Opine.Dispatching
{
    public class MessageContextFactory : IMessageContextFactory
    {
        public MessageContext Create(StoredMessage message)
        {
            return new MessageContext(
                message.MessageId, 
                message.Metadata.AggregateId, 
                message.Metadata.ProcessCode, 
                message.Metadata.ProcessId,
                message.Metadata.MessageDate);
        }
    }
}