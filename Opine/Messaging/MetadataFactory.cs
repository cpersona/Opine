using System;
using Opine.Dispatching;

namespace Opine.Messaging
{
    public class MetadataFactory : IMetadataFactory
    {
        public Metadata Create(MessageContext messageContext)
        {
            return new Metadata(
                messageContext.AggregateId, 
                messageContext.ProcessCode,
                messageContext.ProcessId,
                DateTime.Now);
        }
    }
}