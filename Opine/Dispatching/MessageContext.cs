using System;
using Opine.Domain;

namespace Opine.Dispatching
{
    public class MessageContext
    {
        public MessageContext(Guid messageId, object aggregateId, string processCode, object processId)
        {
            MessageId = messageId;
            AggregateId = aggregateId;
            ProcessCode = processCode;
            ProcessId = processId;
        }

        public Guid MessageId { get; private set; }
        public object AggregateId { get; private set; }
        public string ProcessCode { get; private set; }
        public object ProcessId { get; private set; }
    }
}