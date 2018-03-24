using System;
using Opine.Domain;

namespace Opine.Dispatching
{
    public class MessageContext
    {
        public MessageContext(Guid messageId, object aggregateId, string processCode, object processId, DateTime messageDate, string messageUser)
        {
            this.MessageId = messageId;
            this.AggregateId = aggregateId;
            this.ProcessCode = processCode;
            this.ProcessId = processId;
            this.MessageDate = messageDate;
            this.MessageUser = messageUser;

        }
        public Guid MessageId { get; private set; }
        public object AggregateId { get; private set; }
        public string ProcessCode { get; private set; }
        public object ProcessId { get; private set; }
        public DateTime MessageDate { get; private set; }
        public string MessageUser { get; private set; }
    }
}